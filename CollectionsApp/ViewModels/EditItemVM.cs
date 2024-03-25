namespace CollectionsApp.ViewModels
{
    public class EditItemVM
    {
        public string Id { get; set; } = string.Empty;
     
        public string Name { get; set; } = string.Empty;
       
        public string Tags { get; set; } = string.Empty;
        public Dictionary<string,string> ItemCustomVals { get; set; }= new Dictionary<string,string>(); 
    }
}
