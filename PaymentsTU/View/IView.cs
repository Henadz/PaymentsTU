namespace PaymentsTU
{
    internal interface IView<T>
    {
        T ViewModel { get; set; }
        //object DataContext { get; set; }
    }
}