
public void Main(string argument){
    IMyBlockGroup lift_grp = GridTerminalSystem.GetBlockGroupWithName("pistony winda miner");
    List<IMyPistonBase> pistons = new List<IMyPistonBase>();
    lift_grp.GetBlocksOfType(pistons);
    IMyPistonBase piston_main = GridTerminalSystem.GetBlockWithName("piston winda main") as IMyPistonBase;

    if(argument == "UP miner"){
        foreach(var piston in pistons){
            piston.Velocity = (float)2;
        }
    }

    else if(argument == "DOWN miner"){
        foreach(var piston in pistons){
            piston.Velocity = (float)-2;
        }
    }
    
    else if(argument == "UP mine"){
        piston_main.Velocity = (float)4;
    }

    else if(argument == "DOWN mine"){
        piston_main.Velocity = (float)-4;
    }
}
