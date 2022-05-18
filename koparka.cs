
float max_pos = 0;
float down_pos = 0;

public void Main(string argument){
    
    string height = "";

    if (argument.StartsWith("PARK")){
        string[] arguments = argument.Split('/');
        height = arguments[1];
        Park(float.Parse(height));
    }    
    else if (argument == "SET"){
        Set();
    }    
    else if (argument == "ROTATE"){
        Rotate();
    }
    else if (argument == "STOP MAIN"){
        IMyPistonBase piston_main = GridTerminalSystem.GetBlockWithName("Piston/Mine 3/Ver Main/") as IMyPistonBase;
        piston_main.Velocity = (float)0;
        Echo("stopped");
    }

}

public void Rotate(){
    IMyTimerBlock timer_koparka = GridTerminalSystem.GetBlockWithName("Timer Koparka") as IMyTimerBlock;
    IMyMotorAdvancedStator rotor_main = GridTerminalSystem.GetBlockWithName("Advanced Rotor/Mine 3/") as IMyMotorAdvancedStator;
    IMyShipDrill drill_main = GridTerminalSystem.GetBlockWithName("Drill/Mine 3/") as IMyShipDrill;
    IMyPistonBase piston_hor = GridTerminalSystem.GetBlockWithName("Piston/Mine 3/Hor 1/") as IMyPistonBase;
    IMyPistonBase piston_ver = GridTerminalSystem.GetBlockWithName("Piston/Mine 3/Ver Main/") as IMyPistonBase;
    IMyTextPanel LCD = (IMyTextPanel)GridTerminalSystem.GetBlockWithName("LCD 1"); 
    IMySoundBlock speaker = GridTerminalSystem.GetBlockWithName("speaker") as IMySoundBlock;
        
    piston_hor.MaxLimit = max_pos;
    max_pos += (float)1.4;
    piston_hor.Velocity = (float)0.5;
    drill_main.Enabled = (bool)true;
    rotor_main.TargetVelocityRPM = (float)1;
    timer_koparka.StartCountdown();
    LCD.WriteText("Max pos: " + max_pos.ToString() + "\nVelocity: " + piston_hor.Velocity.ToString() + "\nMax Limit: " + piston_hor.MaxLimit.ToString() + "\nCurrent Pos: " + piston_hor.CurrentPosition.ToString());

    if (max_pos > 10){
        timer_koparka.StopCountdown();
        piston_hor.Velocity = (float)-1;
        max_pos = (float)0;
        piston_hor.MaxLimit = max_pos;
        piston_ver.MaxLimit = down_pos;
        down_pos += (float)1.4;
        piston_hor.Velocity = (float)0.5;
    }

    if (down_pos > 10){
        drill_main.Enabled = (bool)false;
        piston_hor.Velocity = (float)-1;
        piston_ver.Velocity = (float)-1;
        down_pos = (float)0;
        max_pos = (float)0;
        speaker.Play();
        timer_koparka.StopCountdown();
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
