
float max_pos = 0;
float down_pos = 0;
int step = 0;
int steps = 0;

public void Main(string argument){
    
    string[] arguments;
    float height;

    if (argument.StartsWith("PARK")){
        arguments = argument.Split('/');
        height = float.Parse(arguments[1]);
        Park(height);
    }    
    else if (argument == "SET"){
        Set();
    }    
    else if (argument == "ROTATE"){
        StartRotate(steps);
    }
    else if (argument.StartsWith("SKIP")){
        arguments = argument.Split('/');
        steps = Int32.Parse(arguments[1]);
        StartRotate(steps);
    }
    else if (argument == "SOLAR"){
        Solar();
    }
    else if (argument == "STOP"){
        Stop();
    }

}

public void Stop(){
    IMyTimerBlock timer_koparka = GridTerminalSystem.GetBlockWithName("Timer Koparka") as IMyTimerBlock;
    IMyMotorAdvancedStator rotor_main = GridTerminalSystem.GetBlockWithName("Advanced Rotor/Mine 3/") as IMyMotorAdvancedStator;
    IMyShipDrill drill_main = GridTerminalSystem.GetBlockWithName("Drill/Mine 3/") as IMyShipDrill;
    IMyPistonBase piston_hor = GridTerminalSystem.GetBlockWithName("Piston/Mine 3/Hor 1/") as IMyPistonBase;
    IMyTextPanel LCD = (IMyTextPanel)GridTerminalSystem.GetBlockWithName("LCD 1"); 
    IMySoundBlock speaker = GridTerminalSystem.GetBlockWithName("speaker") as IMySoundBlock;

    IMyBlockGroup arm_group = GridTerminalSystem.GetBlockGroupWithName("Piston arm");
    List<IMyPistonBase> arm_pistons = new List<IMyPistonBase>();
    arm_group.GetBlocksOfType(arm_pistons);

    List<IMyPistonBase> arm_pistons_on = new List<IMyPistonBase>();

    foreach(var piston in arm_pistons){
        if(piston.CustomName == "Piston/Mine 3/Ver On/"){
            arm_pistons_on.Add(piston);
        }
    }
    drill_main.Enabled = (bool)false;
    piston_hor.Velocity = (float)-1;
    foreach(var piston in arm_pistons_on){
        piston.Velocity = (float)-1;
    }
    down_pos = (float)0;
    max_pos = (float)0;
    speaker.Play();
    timer_koparka.StopCountdown();
    rotor_main.TargetVelocityRPM = (float)0;
    step = 0;
    LCD.WriteText("Stopped");

}

public void Solar(){
    IMyProgrammableBlock programmable_solar = GridTerminalSystem.GetBlockWithName("Programmable Solary") as IMyProgrammableBlock;
    programmable_solar.Enabled = true;
}

public void StartRotate(int steps){
    if(steps > 0){
        Skip();
    }
    else{
        Rotate();
    }
}

public void Skip(){

    IMyTimerBlock timer_koparka = GridTerminalSystem.GetBlockWithName("Timer Koparka") as IMyTimerBlock;
    steps -= 1;
    bool finish = Rotate(true);
    Echo(finish.ToString());
    if (!finish){
        timer_koparka.Trigger();
    }
}

public bool Rotate(bool skip = false){
    Solar();
    IMyTimerBlock timer_koparka = GridTerminalSystem.GetBlockWithName("Timer Koparka") as IMyTimerBlock;
    IMyMotorAdvancedStator rotor_main = GridTerminalSystem.GetBlockWithName("Advanced Rotor/Mine 3/") as IMyMotorAdvancedStator;
    IMyShipDrill drill_main = GridTerminalSystem.GetBlockWithName("Drill/Mine 3/") as IMyShipDrill;
    IMyPistonBase piston_hor = GridTerminalSystem.GetBlockWithName("Piston/Mine 3/Hor 1/") as IMyPistonBase;
    IMyTextPanel LCD = (IMyTextPanel)GridTerminalSystem.GetBlockWithName("LCD 1"); 
    IMySoundBlock speaker = GridTerminalSystem.GetBlockWithName("speaker") as IMySoundBlock;

    IMyBlockGroup arm_group = GridTerminalSystem.GetBlockGroupWithName("Piston arm");
    List<IMyPistonBase> arm_pistons = new List<IMyPistonBase>();
    arm_group.GetBlocksOfType(arm_pistons);

    List<IMyPistonBase> arm_pistons_on = new List<IMyPistonBase>();

    foreach(var piston in arm_pistons){
        if(piston.CustomName == "Piston/Mine 3/Ver On/"){
            arm_pistons_on.Add(piston);
        }
    }

    int steps_to_go = (int)(10/1.5) * (int)(10/(1.5/arm_pistons_on.Count));
    step += 1;
    int eta_h = (steps_to_go - step) / 60;
    int eta_m = (steps_to_go - step) % 60;

    piston_hor.MaxLimit = max_pos;
    max_pos += (float)1.5;
    piston_hor.Velocity = (float)0.5;
    drill_main.Enabled = (bool)true;
    rotor_main.TargetVelocityRPM = (float)1;

    if(!skip){
        timer_koparka.StartCountdown();
    }
    
    IMyPistonBase first_piston = arm_pistons_on[0];
    string text = "Horizontal:";
    text += "\n    Max pos: " + max_pos.ToString();
    text += "\n    Velocity: " + piston_hor.Velocity.ToString();
    text += "\n    Max Limit: " + piston_hor.MaxLimit.ToString();
    text += "\n    Current Pos: " + piston_hor.CurrentPosition.ToString();
    text += "\nVertical:";
    text += "\n    Max pos: " + down_pos.ToString();
    text += "\n    Velocity: " + first_piston.Velocity.ToString();
    text +=  "\n    Max Limit: " + first_piston.MaxLimit.ToString();
    text += "\n    Current Pos: " + first_piston.CurrentPosition.ToString();
    text += "\n    Piston Count: " + arm_pistons_on.Count;
    text += "\n    Arm Length: " + (down_pos*arm_pistons_on.Count).ToString();
    text += "\nStep: " + step.ToString() + "/" + steps_to_go.ToString();
    text += "\nETA: " + eta_h.ToString() + ":" + eta_m.ToString();
    LCD.WriteText(text);

    if (max_pos > 10){
        piston_hor.Velocity = (float)-1;
        max_pos = (float)0;
        piston_hor.MaxLimit = max_pos;

        down_pos += ((float)1.5/arm_pistons_on.Count);
        foreach(var piston in arm_pistons_on){
            piston.MaxLimit = down_pos;
            piston.Velocity = (float)0.5;
        }
    }

    if (down_pos > 10){
        drill_main.Enabled = (bool)false;
        piston_hor.Velocity = (float)-1;
        foreach(var piston in arm_pistons_on){
            piston.Velocity = (float)-1;
        }
        down_pos = (float)0;
        max_pos = (float)0;
        speaker.Play();
        timer_koparka.StopCountdown();
        rotor_main.TargetVelocityRPM = (float)0;
        LCD.WriteText("Mining Complete");
        step = 0;
        return true;
    }
    else{
        return false;
    }
}

public void Set(){

    IMyBlockGroup hinge_group = GridTerminalSystem.GetBlockGroupWithName("hinge");
    List<IMyMotorStator> hinges = new List<IMyMotorStator>();
    hinge_group.GetBlocksOfType(hinges);
    IMyMotorStator rotor_solar1 = GridTerminalSystem.GetBlockWithName("Rotor Solar hor") as IMyMotorStator;
    IMyMotorStator rotor_solar2 = GridTerminalSystem.GetBlockWithName("Rotor Solar ver 1") as IMyMotorStator;
    IMyMotorStator rotor_solar3 = GridTerminalSystem.GetBlockWithName("Rotor Solar ver 2") as IMyMotorStator;
    IMyProgrammableBlock programmable_solar = GridTerminalSystem.GetBlockWithName("Programmable Solary") as IMyProgrammableBlock;
    IMyBlockGroup holder_group = GridTerminalSystem.GetBlockGroupWithName("holdery");
    List<IMyShipConnector> connectors = new List<IMyShipConnector>();
    holder_group.GetBlocksOfType(connectors);
    IMyPistonBase holder_piston = GridTerminalSystem.GetBlockWithName("Piston holder") as IMyPistonBase;
    IMyMotorStator holder_hinge = GridTerminalSystem.GetBlockWithName("Hinge holder") as IMyMotorStator;
    IMyBlockGroup arm_group = GridTerminalSystem.GetBlockGroupWithName("Piston Inv");
    List<IMyPistonBase> arm_pistons = new List<IMyPistonBase>();
    arm_group.GetBlocksOfType(arm_pistons);
 
   foreach(var connector in connectors){
        connector.Disconnect();
    }
    
    holder_piston.Velocity = (float)-0.3;
    holder_hinge.TargetVelocityRPM = (float)1;

    programmable_solar.Enabled = (bool)false;
    rotor_solar1.TargetVelocityRPM = (float)1;
    rotor_solar1.UpperLimitDeg = (float)90;
    rotor_solar2.RotorLock = (bool)true;
    rotor_solar3.RotorLock = (bool)true;

    foreach(var hinge in hinges){
        hinge.TargetVelocityRPM = (float)0.3;
    }

    foreach(var piston in arm_pistons){
        piston.Velocity = (float)0.1;
    }

}

public void Park(float height){
    
    IMyBlockGroup dock_group = GridTerminalSystem.GetBlockGroupWithName("Piston dock");
    List<IMyPistonBase> pistons = new List<IMyPistonBase>();
    dock_group.GetBlocksOfType(pistons);

    foreach(var piston in pistons){
        piston.MaxLimit = height;
        piston.Velocity = (float)0.3;
        piston.Extend();
    }

    IMyTextPanel LCD = (IMyTextPanel)GridTerminalSystem.GetBlockWithName("LCD 1"); 
    LCD.WriteText("Parked");
}
