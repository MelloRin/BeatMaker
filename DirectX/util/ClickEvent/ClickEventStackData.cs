namespace DirectX.util.ClickEvent
{
    public class ClickEventStackData<E>
    {
        public ClickEventStackData(E data) { this.data = data; }

        private readonly E data;
        public E getData() { return data; }

        private ClickEventStackData<E> nextData;

        public void setNext(ClickEventStackData<E> nextData) { this.nextData = nextData; }
        public ClickEventStackData<E> getNext() { return nextData; }
    }
}
