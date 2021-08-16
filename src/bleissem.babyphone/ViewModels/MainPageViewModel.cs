using bleissem.babyphone.Core.Events;
using bleissem.babyphone.Core.Messages;
using Prism.Events;
using Prism.Navigation;

namespace bleissem.babyphone.ViewModels
{
    public class MainPageViewModel : ViewModelBase
    {
        private readonly IEventAggregator _eventAggregator;
        private SubscriptionToken _audioRecordSubscriptionToken;

        private string _selectedContact;

        public string SelectedContact
        {
            get
            {
                return _selectedContact;
            }
            set
            {
                SetProperty(ref _selectedContact, value);
            }
        }

        private string _selectedYouAreUsing;

        public string SelectedYouAreUsing
        {
            get
            {
                return _selectedYouAreUsing;
            }
            set
            {
                SetProperty(ref _selectedYouAreUsing, value);
            }
        }

        private string _noiseLevel;

        public string NoiseLevel
        {
            get
            {
                return _noiseLevel;
            }
            set
            {
                SetProperty(ref _noiseLevel, value);
            }
        }

        public MainPageViewModel(INavigationService navigationService, IEventAggregator eventAggregator)
            : base(navigationService)
        {
            _eventAggregator = eventAggregator;
            _audioRecordSubscriptionToken = _eventAggregator.GetEvent<AudioRecordEvent>().Subscribe(OnAudioRecordMessage);
        }

        private void OnAudioRecordMessage(AudioRecordMessage message)
        {
            NoiseLevel = message.Level.ToString();
        }

        public override void Destroy()
        {
            _audioRecordSubscriptionToken.Dispose();
            _audioRecordSubscriptionToken = null;
            base.Destroy();
        }
    }
}