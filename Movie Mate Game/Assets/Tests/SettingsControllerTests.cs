using System;
using DefaultNamespace;
using DefaultNamespace.OptionsSelector;
using NSubstitute;
using NUnit.Framework;
using Settings;

namespace Tests
{
    public class SettingsControllerTests
    {
        private ISettingsView _view;
        private ILanguageToggle _langToggle;
        private IResolutionSelector _resSelector;
        private IApiService _api;
        private ILocalizationService _loc;

        [SetUp]
        public void SetUp()
        {
            _view = Substitute.For<ISettingsView>();
            _langToggle = Substitute.For<ILanguageToggle>();
            _resSelector = Substitute.For<IResolutionSelector>();
            _api = Substitute.For<IApiService>();
            _loc = Substitute.For<ILocalizationService>();
        }

        [TearDown]
        public void TearDown()
        {
            _view = null;
            _langToggle = null;
            _resSelector = null;
            _api = null;
            _loc = null;
        }
        
        [Test]
        public void OnLanguageChanged_ApiService_SetsNewLanguageCode()
        {
            // Arrange
            var sut = new SettingsController(_view, _langToggle, _resSelector, _api, _loc);
            _loc.GetCurrentLanguageCode().Returns("ru-RU");

            // Act
            sut.InitializeSettings();
            _langToggle.OnLanguageChanged += Raise.Event<Action<LocalizationLanguage>>(LocalizationLanguage.Russian);
            
            // Assert
            _api.Received(1).SetLanguage("ru-RU");
        }

        [Test]
        public void OnResolutionChanged_ApiService_SetsNewResolution()
        {
            // Arrange
            var sut = new SettingsController(_view, _langToggle, _resSelector, _api, _loc);
            const int midRes = 1;
            const string midResString = "w500";
            
            // Act
            sut.InitializeSettings();
            _resSelector.OnResolutionChanged += Raise.Event<Action<int>>(midRes);
            
            // Assert
            _api.Received(1).SetImageResolution(midResString);
        }

        [Test]
        public void OnGoBackRequested_Controller_Raises_OnBackClicked()
        {
            // Arrange
            var sut = new SettingsController(_view, _langToggle, _resSelector, _api, _loc);
            var closeRequested =  false;
            
            // Act
            sut.OnGoBackRequested += HandleGoBackRequest;
            _view.OnBackClicked += Raise.Event<Action>();
            
            // Assert
            Assert.IsTrue(closeRequested);
            return;

            void HandleGoBackRequest()
            {
                closeRequested = true;
            }
        }
    }
}