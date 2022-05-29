List<IMyCargoContainer> cargo_main = new List<IMyCargoContainer>();
List<IMyCargoContainer> cargo_ore = new List<IMyCargoContainer>();

public Program(){
    Runtime.UpdateFrequency = UpdateFrequency.Update100;
}

public void Main(string argument, UpdateType updateSource){
    GridTerminalSystem.GetBlocksOfType<IMyCargoContainer>(cargo_main, c => c.CustomName.Contains("[MAIN]"));
    GridTerminalSystem.GetBlocksOfType<IMyCargoContainer>(cargo_ore, c => c.CustomName.Contains("[ORE"));
    IMyProgrammableBlock miner = GridTerminalSystem.GetBlockWithName("Programmable Block Miner") as IMyProgrammableBlock;
    IMyTextSurface LCD = Me.GetSurface(0);

    float main_used_volume = 0;
    float main_max_volume = 0;
    for(int i = 0; i < cargo_main.Count(); i++){
        var container = cargo_main[i];
        main_used_volume += (float)container.GetInventory(0).CurrentVolume;
        main_max_volume += (float)container.GetInventory(0).MaxVolume;
    }

    float ore_used_volume = 0;
    float ore_max_volume = 0;
    for(int i = 0; i < cargo_ore.Count(); i++){
        var container = cargo_main[i];
        ore_used_volume += (float)container.GetInventory(0).CurrentVolume;
        ore_max_volume += (float)container.GetInventory(0).MaxVolume;
    }

    if((ore_used_volume > (ore_max_volume/2)) | (main_used_volume > (main_max_volume/1.3))){
        if(!miner.GetSurface(0).GetText().Contains("Paused")){
            miner.TryRun("pause");
        }
        LCD.WriteText("Miner Status:\nPaused");
    }
    else{
        if(miner.GetSurface(0).GetText().Contains("Paused")){
            miner.TryRun("start");
            LCD.WriteText("Miner Status:\nStarted");
        }
        else if(miner.GetSurface(0).GetText().Contains("Mining Complete")){
            LCD.WriteText("Miner Status:\nMining Complete");
        }
        else{
            LCD.WriteText("Miner Status:\nMining");
        }
    }
}
