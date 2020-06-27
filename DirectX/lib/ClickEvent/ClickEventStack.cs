namespace DirectX.lib.ClickEvent
{
    public class ClickEventStack<E>
    {
        public ClickEventStackData<E> head { get; private set; }

        public void push(E next)
        {
            ClickEventStackData<E> packedData = new ClickEventStackData<E>(next);

            if (head == null)
                head = packedData;
            else
            {
                packedData.setNext(head);
                head = packedData;
            }
        }
    }
}
