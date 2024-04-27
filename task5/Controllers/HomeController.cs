using Bogus;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Newtonsoft.Json;
using System.Reflection;
using task5.Models;
using task5.Models.ViewModel;

namespace task5.Controllers
{
    public class HomeController(ILogger<HomeController> logger, IMemoryCache memoryCache) : Controller
    {
        private IMemoryCache _cache = memoryCache;
        private readonly ILogger<HomeController> _logger = logger;

        public IActionResult Index()
        {
            UserViewModel viewModel = new UserViewModel();
            return View(viewModel);
        }

        [HttpPost]
        public IActionResult LoadMoreData(UserViewModel model)
        {
            model.seed += model.page*10 -10;

            if(model.users != null) {
                model.users.AddRange(GenerateFakeData(model.selectedRegion, model.errorCountVal, model.seed)); }
            else {
                model.users = GenerateFakeData(model.selectedRegion, model.errorCountVal, model.seed);
            }
            return View("Index", model);
        }

        [HttpPost]
        public IActionResult GenerateData(UserViewModel model)
        {
            model.users = GenerateFakeData(model.selectedRegion, model.errorCountVal, model.seed);
            return View("Index", model);
        }

        private List<User> GenerateFakeData(string selectedRegion, float errorCount, int seed)
        {
            Dictionary<string, string> languageCharacterSets = new Dictionary<string, string>
            {
                { "ru", "абвгдеёжзийклмнопрстуфхцчшщъыьэюя1234567890" },  // Russian character set
                { "en_US", "abcdefghijklmnopqrstuvwxyz1234567890" },  // English character set
                { "pl", "a?bc?de?fghijkl?mn?o?prs?tuwyz??1234567890" }  // Polish character set
                };
            string seedRegion;
            switch (selectedRegion)
            {
                case "Россия":
                    seedRegion = "ru";
                    break;
                case "USA":
                    seedRegion = "en_US";
                    break;
                case "Polska":
                    seedRegion = "pl";
                    break;
                default:
                    seedRegion = "ru";
                    break;
            }
            var fakeData = new List<User>();

            var faker = new Faker<User>(seedRegion).UseSeed(seed)
                .RuleFor(u => u.Name, f => f.Name.FullName())
                .RuleFor(u => u.Email, f => f.Internet.Email())
                .RuleFor(u => u.Address, f => f.Address.StreetAddress())
                .RuleFor(u => u.Phone, f => f.Phone.PhoneNumber())
                .FinishWith((f, u) =>
                {
                    if (errorCount > 0)
                    {
                        // Увеличиваем errorCount на 1 с вероятностью, равной дробной части
                        int integerPart = (int)errorCount;
                        double decimalPart = errorCount - integerPart;
                        if (decimalPart > 0)
                        {
                           
                            Random rand = new Random();
                            if (rand.NextDouble() <= decimalPart)
                            {
                                integerPart++;
                            }
                        }
                        errorCount = integerPart;

                        var errorGenerator = new Random();

                        foreach (var fieldInfo in typeof(User).GetProperties())
                        {
                            var fieldValue = fieldInfo.GetValue(u, null);
                            if (fieldValue != null && fieldValue is string)
                            {
                                var fieldName = fieldInfo.Name;
                                for (int i = 0; i < errorCount; i++)
                                {
                                    var field = errorGenerator.Next(0, 3);
                                    switch (field)
                                    {
                                    case 0: // Delete one random character
                                        if (!string.IsNullOrEmpty(fieldValue as string))
                                        {
                                         int indexToDelete = errorGenerator.Next(0, (fieldValue as string).Length);
                                         fieldValue = (fieldValue as string).Remove(indexToDelete, 1);
                                        }
                                    break;
                                    case 1: // Insert one random character
                                         if (!string.IsNullOrEmpty(fieldValue as string))
                                         {
                                             int indexToInsert = errorGenerator.Next(0, (fieldValue as string).Length);
                                             string languageCharacterSet = languageCharacterSets[seedRegion];
                                             char randomChar = languageCharacterSet[errorGenerator.Next(0, languageCharacterSet.Length)];
                                             fieldValue = (fieldValue as string).Insert(indexToInsert, randomChar.ToString());
                                         }
                                         break;
                                    case 2: // Swap two adjacent characters
                                         if (!string.IsNullOrEmpty(fieldValue as string) && (fieldValue as string).Length > 1)
                                         {
                                             int indexToSwap = errorGenerator.Next(0, (fieldValue as string).Length - 1);
                                             var chars = (fieldValue as string).ToCharArray();
                                             char temp = chars[indexToSwap];
                                             chars[indexToSwap] = chars[indexToSwap + 1];
                                             chars[indexToSwap + 1] = temp;
                                             fieldValue = new string(chars);
                                         }
                                         break;
                                    }
                                }

                                fieldInfo.SetValue(u, fieldValue);
                            }
                        }
                    }
                });

            for (int i = 0; i < 10; i++)
            {
                fakeData.Add(faker.Generate());
            }

            return fakeData;
        }
    }
}
