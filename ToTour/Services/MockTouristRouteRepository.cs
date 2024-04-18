//using ToTour.Models;

//namespace ToTour.Services
//{
//    public class MockTouristRouteRepository : ITouristRouteRepository
//    {
//        private List<TouristRoute>? _routes;
//        public MockTouristRouteRepository()
//        {
//            if (_routes == null)
//            {
//                InitializeTouristRoutes();
//            }
//        }
//        private void InitializeTouristRoutes()
//        {
//            _routes = new List<TouristRoute>
//            {
//                new TouristRoute
//                {
//                    Id = Guid.NewGuid(),
//                    Title =  "黄山",
//                    Description = "黄山真好玩！",
//                    OriginalPrice = 998,
//                    Features = "<p>吃住行旅游</p>",
//                    Fees = "<p>交通费用自理</p>",
//                    Notes = "<p>小心危险</p>"
//                }
//            };
//        }

//        public TouristRoute GetTouristRoute(Guid touristRouteId)
//        {
//            return _routes.FirstOrDefault(n => n.Id == touristRouteId);
//        }

//        public IEnumerable<TouristRoute> GetTouristRoutes()//string keyword, string operatorType, int? raringVlaue)
//        {
//            return _routes;
//        }
//    }
//}
