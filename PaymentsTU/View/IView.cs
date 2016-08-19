namespace PaymentsTU
{
    internal interface IView<T>
    {
        T Model { get; set; }
        object DataContext { get; set; }
    }
}