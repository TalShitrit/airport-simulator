using CommonServiceLocator;
using GalaSoft.MvvmLight.Ioc;


namespace AirportClient.ViewModel
{
    public class ViewModelLocator
    {
        public ViewModelLocator()
        {
            ServiceLocator.SetLocatorProvider(() => SimpleIoc.Default);
            SimpleIoc.Default.Register<MainViewModel>();
        }
        public MainViewModel Main => ServiceLocator.Current.GetInstance<MainViewModel>();
    }
}