public class InventorySlot
{
    public ItemSO item;
    public int count;

    public bool IsEmpty => item == null;
}