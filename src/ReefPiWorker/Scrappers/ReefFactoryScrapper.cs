﻿using OpenQA.Selenium.Chrome;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;
using ReefPiWorker.Scrappers.Models;
using Microsoft.Extensions.Options;

namespace ReefPiWorker.Scrappers
{
    public class ReefFactoryScrapper : IReefFactoryScrapper
    {
        private readonly ILogger<ReefFactoryScrapper> _logger;
        private readonly ReefFactoryScrapperOptions _options;

        private readonly string _reefFactoryWebUrl;

        public ReefFactoryScrapper(
            ILogger<ReefFactoryScrapper> logger,
            IOptions<ReefFactoryScrapperOptions> options)
        {
            (_logger, _options) = (logger, options.Value);

            _reefFactoryWebUrl = $"https://{_options.RfUrl}/";
        }

        private void LoginAndAcceptCookies(ref IWebDriver driver, ref WebDriverWait wait)
        {
            driver.Navigate().GoToUrl($"{_reefFactoryWebUrl}?state=login");
            wait.Until(SeleniumExtras.WaitHelpers.ExpectedConditions.ElementExists(By.Id("userName")));

            var userName = driver.FindElement(By.Id("userName"));
            userName.Clear();
            userName.SendKeys(_options.RfUserName);
            var userLoginPwd = driver.FindElement(By.Id("userLoginPwd"));
            userLoginPwd.Clear();
            userLoginPwd.SendKeys(_options.RfPassword);
            var loginButton = driver.FindElement(By.Id("userButtonLogin"));
            loginButton.Click();

            wait.Until(SeleniumExtras.WaitHelpers.ExpectedConditions.ElementExists(By.ClassName("cookiesClose")));
            var cookiesClose = driver.FindElement(By.ClassName("cookiesClose"));
            cookiesClose.Click();
        }

        private static IWebDriver CreateChromeWebDriver()
        {
            var service = ChromeDriverService.CreateDefaultService();
            service.EnableVerboseLogging = false;
            service.SuppressInitialDiagnosticInformation = true;
            service.HideCommandPromptWindow = true;

            var options = new ChromeOptions
            {
                PageLoadStrategy = PageLoadStrategy.Normal
            };
            options.AddArgument("--window-size=1920,1080");
            options.AddArgument("--no-sandbox");
            options.AddArgument("--headless");
            options.AddArgument("--disable-gpu");
            options.AddArgument("--disable-crash-reporter");
            options.AddArgument("--disable-extensions");
            options.AddArgument("--disable-in-process-stack-traces");
            options.AddArgument("--disable-logging");
            options.AddArgument("--disable-dev-shm-usage");
            options.AddArgument("--log-level=3");
            options.AddArgument("--output=/dev/null");

            IWebDriver driver = new ChromeDriver(service, options);

            return driver;
        }

        public Task<ReefFactoryKhKeeperPlusDataModel?> ReadLastKhPhValues()
        {
            try
            {
                var driver = CreateChromeWebDriver();
                var wait = new WebDriverWait(driver, TimeSpan.FromSeconds(2));

                LoginAndAcceptCookies(ref driver, ref wait);

                wait.Until(SeleniumExtras.WaitHelpers.ExpectedConditions.ElementExists(By.Id("hardwareName3")));
                var hardware = driver.FindElement(By.Id("hardwareName3"));
                hardware.Click();

                wait.Until(
                    SeleniumExtras.WaitHelpers.ExpectedConditions.InvisibilityOfElementLocated(
                        By.Id("componentLoading")));

                wait.Until(d => d.FindElement(By.Id("rfkh01KhValue")).Text != "-.--");

                var phText = driver.FindElement(By.Id("rfkh01PhValue")).Text.Replace("pH: ", "").Trim()
                    .Replace(".", ",");
                var khText = driver.FindElement(By.Id("rfkh01KhValue")).Text.Trim().Replace(".", ",");
                var dateTimeText = driver.FindElement(By.Id("rfkh01KhStatus")).Text.Replace("Measurement time: ", "")
                    .Trim().Replace(".", ",");

                driver.Quit();

                //'Measurement in progress: 0 % '
                if (dateTimeText.Contains("progress"))
                {
                    return Task.FromResult<ReefFactoryKhKeeperPlusDataModel?>(null);
                }
                else
                {
                    var data = new ReefFactoryKhKeeperPlusDataModel
                    {
                        OnDateTimeUtc = DateTime.Parse(dateTimeText).ToUniversalTime(),
                        Kh = double.Parse(khText),
                        Ph = double.Parse(phText)
                    };

                    return Task.FromResult(data)!;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError("Error getting data from ReefFactory webpage", ex);
                return Task.FromResult<ReefFactoryKhKeeperPlusDataModel?>(null);
            }
        }
    }
}
