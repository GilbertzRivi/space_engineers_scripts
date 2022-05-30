List<IMyCargoContainer> cargo_main = new List<IMyCargoContainer>();
int limit = 200000;

public Program(){
    Runtime.UpdateFrequency = UpdateFrequency.Update100;
}

public void Main(string argument, UpdateType updateSource){
    GridTerminalSystem.GetBlocksOfType<IMyCargoContainer>(cargo_main, c => c.CustomName.Contains("[MAIN]"));
    IMyConveyorSorter sorter_gravel = GridTerminalSystem.GetBlockWithName("Conveyor Sorter [GRAVEL]") as IMyConveyorSorter;
    IMyTextSurface LCD = GridTerminalSystem.GetBlockWithName("LCD GRAVEL") as IMyTextSurface;

    int gravel_amount = 0;

    for(int i = 0; i < cargo_main.Count(); i++){
        var cargo_b = cargo_main[i];
        gravel_amount += cargo_b.GetInventory().GetItemAmount(MyItemType.MakeIngot("Stone")).ToIntSafe();
    }

    if(gravel_amount > limit){
        sorter_gravel.Enabled = true;
    }
    else{
        sorter_gravel.Enabled = false;
    }

    float gravel_percent = gravel_amount/(limit/100);
    string text = "";
    text += "Set amount: " + (limit/1000).ToString() + "k\n";
    text += "Gravel amount: " + (gravel_amount/1000).ToString() + "k\n";
    text += gravel_percent.ToString() + "% of set amount";

    LCD.WriteText(text);
}
