namespace AgravitaeWebExtension.Models.GenericReports
{
    public class SaveItemInfo
    {
        public MyItem[] Items { get; set; }
        public SortOrderInfo[] Sort { get; set; }
        public ColumnIndexInfo[] ColInfo { get; set; }
    }
}
