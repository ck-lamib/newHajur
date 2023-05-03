

using Microsoft.AspNetCore.Identity;
using HajurKoCarRental.Models;
using HajurKoCarRental.Areas.Identity.Data;
using System.Diagnostics.Metrics;

namespace HajurKoCarRental.Data;

public class ApplicationDBInitilizer
{
    public static async Task SeedUsersAndRolesAsync(IApplicationBuilder applicationBuilder)
    {
        using (var serviceScope = applicationBuilder.ApplicationServices.CreateScope())
        {
            // Roles
            var roleManager = serviceScope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();


            if (!await roleManager.RoleExistsAsync(UserRoles.Admin))
                await roleManager.CreateAsync(new IdentityRole(UserRoles.Admin));

            if (!await roleManager.RoleExistsAsync(UserRoles.Staff))
                await roleManager.CreateAsync(new IdentityRole(UserRoles.Staff));

            if (!await roleManager.RoleExistsAsync(UserRoles.Customer))
                await roleManager.CreateAsync(new IdentityRole(UserRoles.Customer));
            // Users
            var userManager = serviceScope.ServiceProvider.GetRequiredService<UserManager<HajurKoCarRentalUser>>();

            var adminUserEmail = "atishgurung13@gmail.com";
            var adminUser = await userManager.FindByEmailAsync(adminUserEmail);
            if (adminUser == null)
            {
                var newAdminUser = new HajurKoCarRentalUser
                {
                    UserName = "atishgurung13@gmail.com",
                    FullName = "AdminAtish",
                    Email = adminUserEmail,
                    Address = "Pokhara",
                    PhoneNumber = "9829172294",
                    SecurityStamp = Guid.NewGuid().ToString(),
                    EmailConfirmed = true
                };
                await userManager.CreateAsync(newAdminUser, "Bimal@12");
                await userManager.AddToRoleAsync(newAdminUser, UserRoles.Admin);
            }

        }
    }



    public static void Seed(IApplicationBuilder applicationBuilder)
    {
        using (var serviceScope = applicationBuilder.ApplicationServices.CreateScope())
        {
            var context = serviceScope.ServiceProvider.GetService<HajurKoCarRentalDbContext>();
            context.Database.EnsureCreated();


            // Cars
            if (!context.CarInfo.Any())
            {
                context.CarInfo.AddRangeAsync(new List<CarInfo>
                {
                    new()
                    {
                    CarName= "Toyata Camry",

                    CarBrand = "Toyota",
                    CarDescription = "A mid-size car that's reliable and efficient.",
                    CarImage = "https://cdn.pixabay.com/photo/2016/12/03/18/57/amg-1880381_960_720.jpg",
                    CarModel = "Camry",
                    CarNumber = "AB1234",
                    RentPrice = 1000,
                    is_available = true
                    },
                    new()
                    {
                       CarName = "Honda Civic",
                       CarBrand = "Honda",
                       CarDescription = "A compact car that's fun to drive and easy on gas.",
                       CarImage = "https://imgd.aeplcdn.com/664x374/n/cw/ec/27074/civic-exterior-right-front-three-quarter-148156.jpeg?q=75",
                       CarModel = "Civic",
                       CarNumber = "CD5678",
                       RentPrice = 900,
                       is_available = true
                    },
                   new()
                   {

                       CarName = "Ford Mustang",
                       CarBrand = "Ford",
                       CarDescription = "A classic muscle car with a powerful engine.",
                       CarImage = "https://imgd.aeplcdn.com/664x374/cw/ec/23766/Ford-Mustang-Exterior-126883.jpg?wm=0&q=75",
                       CarModel = "Mustang",
                       CarNumber = "EF9012",
                       RentPrice = 1500,
                       is_available = true
                   },
                   new()
                   {

                       CarName = "Ford Mustang",
                       CarBrand = "Ford",
                       CarDescription = "A classic muscle car with a powerful engine.",
                       CarImage = "https://imgd.aeplcdn.com/664x374/cw/ec/23766/Ford-Mustang-Exterior-126883.jpg?wm=0&q=75",
                       CarModel = "Mustang",
                       CarNumber = "EF9012",
                       RentPrice = 1500,
                       is_available = true
                   },


                    new ()
                    {
                        CarName = "Mercedes-Benz S-Class",
                        CarBrand = "Mercedes-Benz",
                        CarDescription = "A luxury sedan with advanced technology and a comfortable ride.",
                        CarImage = "https://imgd.aeplcdn.com/664x374/n/cw/ec/48067/s-class-exterior-right-front-three-quarter-3.jpeg?q=75",
                        CarModel = "S-Class",
                        CarNumber = "KL1234",
                        RentPrice = 3000,
                        is_available = true
                    },
                    new()
                    {
                        CarName = "BMW 7 Series",
                        CarBrand = "BMW",
                        CarDescription = "Another luxury sedan with great handling and a smooth ride.",
                        CarImage = "https://imgd.aeplcdn.com/664x374/n/cw/ec/132513/new-7-series-exterior-right-front-three-quarter-2.jpeg?isig=0&q=75",
                        CarModel = "BMW",
                        CarNumber = "BMW1234",
                        RentPrice = 3000,
                        is_available = true
                    },
                    new()
                    {
                        CarName = "Toyota Corolla",
                        CarBrand = "Toyota",
                        CarDescription = "A reliable and spacious sedan with great fuel efficiency.",
                        CarImage = "https://carsales.pxcrush.net/car/spec/S000BHAO.jpg?pxc_method=gravityfill&pxc_bgtype=self&pxc_size=720,480&watermark=2037402514",
                        CarModel = "Corolla",
                        CarNumber = "TC4567",
                        RentPrice = 950,
                        is_available = true
                    },
                    new()
                    {
                        CarName = "Audi A4",
                        CarBrand = "Audi",
                        CarDescription = "A luxurious sedan with advanced technology and a smooth ride.",
                        CarImage = "https://cdni.autocarindia.com/Utils/ImageResizer.ashx?n=https://cms.haymarketindia.net/model/uploads/modelimages/Audi-A4-190120211207.jpg&w=373&h=245&q=75&c=1",
                        CarModel = "A4",
                        CarNumber = "AD6789",
                        RentPrice = 1800,
                        is_available = true
                    },
                    new()
                    {
                        CarName = "Hyundai Tucson",
                        CarBrand = "Hyundai",
                        CarDescription = "A versatile and spacious SUV with modern features.",
                        CarImage = "https://imgd.aeplcdn.com/664x374/n/cw/ec/106821/new-tucson-exterior-right-front-three-quarter-5.jpeg?isig=0&q=75",
                        CarModel = "Tucson",
                        CarNumber = "HT7890",
                        RentPrice = 1200,
                        is_available = true
                    },
                    new()
                    {
                        CarName = "BMW X3",
                        CarBrand = "BMW",
                        CarDescription = "A sporty SUV with excellent handling and advanced technology.",
                        CarImage = "https://imgd.aeplcdn.com/664x374/n/cw/ec/110503/x3-facelift-exterior-right-front-three-quarter.jpeg?isig=0&q=75",
                        CarModel = "X3",
                        CarNumber = "BX8901",
                        RentPrice = 2200,
                        is_available = true
                    },
                    new()
                    {
                        CarName = "Ford Mustang",
                        CarBrand = "Ford",
                        CarDescription = "A classic American muscle car with bold styling and powerful performance.",
                        CarImage = "https://imgd.aeplcdn.com/664x374/cw/ec/23766/Ford-Mustang-Exterior-126883.jpg?wm=0&q=75",
                        CarModel = "Mustang",
                        CarNumber = "FM9012",
                        RentPrice = 1700,
                        is_available = true
                    },
                    new()
                    {
                        CarName = "Jeep Wrangler",
                        CarBrand = "Jeep",
                        CarDescription = "An iconic SUV with off-road capabilities and a rugged exterior.",
                        CarImage = "https://imgd.aeplcdn.com/1056x594/n/cw/ec/54437/2021-wrangler-exterior-right-front-three-quarter.jpeg?q=75&wm=1",
                        CarModel = "Wrangler",
                        CarNumber = "JW0123",
                        RentPrice = 1500,
                        is_available = true
                    },
                    new()
                    {
                        CarName = "Nissan Altima",
                        CarBrand = "Nissan",
                        CarDescription = "A spacious and comfortable sedan with good fuel economy.",
                        CarImage = "https://ymimg1.b8cdn.com/resized/car_model/8379/pictures/9037408/webp_listing_main_webp_listing_main_14841_st1280_046.webp",
                        CarModel = "Altima",
                        CarNumber = "NA2345",
                        RentPrice = 1000,
                        is_available = true
                    },
                    new()
                    {
                        CarName = "Hyundai Sonata",
                        CarBrand = "Hyundai",
                        CarDescription = "A midsize sedan with modern features and a comfortable ride.",
                        CarImage = "https://cache3.pakwheels.com/system/car_generation_pictures/6412/original/Hyundai-Sonata-Front.jpg?1651053886",
                        CarModel = "Sonata",
                        CarNumber = "HD6789",
                        RentPrice = 850,
                        is_available = true
                    }
                });
                context.SaveChanges();
            }

        }
    }
}


