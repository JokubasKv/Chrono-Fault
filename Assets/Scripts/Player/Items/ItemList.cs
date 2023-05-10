[System.Serializable]
public class ItemList
{
    public ItemAbstract item;
    public ItemData data;
    public int stacks;

    public ItemList(ItemAbstract item, ItemData itemData, int stacks)
    {
        this.item = item;
        this.data = itemData;
        this.stacks = stacks;
    }
}
