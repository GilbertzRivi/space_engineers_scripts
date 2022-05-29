
public void Main(string argument){
    IMyBlockGroup lift_grp = GridTerminalSystem.GetBlockGroupWithName("pistony winda");
    List<IMyPistonBase> pistons = new List<IMyPistonBase>();
    lift_grp.GetBlocksOfType(pistons);

    if(argument == "UP"){
        foreach(var piston in pistons){
            piston.Velocity = (float)2;
        }
    }

    else if(argument == "DOWN"){
        foreach(var piston in pistons){
            piston.Velocity = (float)-2;
        }
    }
}
