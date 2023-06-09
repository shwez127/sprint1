﻿
using DeskEntity.Model;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace DeskUI.Controllers
{
    public class EmployeeController : Controller
    {
        IConfiguration _configuration;

        public EmployeeController(IConfiguration configuration)
        {
            _configuration = configuration;

        }

        public static BookingSeat bookingSeat1 = new BookingSeat();
        public IActionResult Index()
        {
            return View();
        }

        public async Task<IActionResult> Profile()
        {
            #region Employee profile
            //storing the profile Id
            int EmployeeProfileId = Convert.ToInt32(TempData["ProfileID"]);
            TempData.Keep();

            Employee employee = null;
            using (HttpClient client = new HttpClient())
            {
                string endpoint = _configuration["WebApiBaseUrl"] + "Employee/GetEmployeeById?employeeId=" + EmployeeProfileId;
                using (var response = await client.GetAsync(endpoint))
                {
                    if (response.StatusCode == System.Net.HttpStatusCode.OK)
                    {
                        var result = await response.Content.ReadAsStringAsync();
                        employee = JsonConvert.DeserializeObject<Employee>(result);
                    }
                }
            }
            return View(employee);
            #endregion
        }

        public IActionResult AddEmployee()
        {
            return View();

        }


        [HttpPost]
        public async Task<IActionResult> AddEmployee(Employee employee)
        {
            #region Adding employee Post Method
            try
            {
                ViewBag.status = "";

                //using grabage collection only for inbuilt classes
                using (HttpClient client = new HttpClient())
                {
                    StringContent content = new StringContent(JsonConvert.SerializeObject(employee), Encoding.UTF8, "application/json");
                    string endPoint = _configuration["WebApiBaseUrl"] + "Employee/AddEmployee";//api controller name and its function

                    using (var response = await client.PostAsync(endPoint, content))
                    {
                        if (response.StatusCode == System.Net.HttpStatusCode.OK)
                        {   //dynamic viewbag we can create any variable name in run time
                            ViewBag.status = "Ok";
                            ViewBag.message = "Employee Added Successfull!!";
                        }

                        else
                        {
                            ViewBag.status = "Error";
                            ViewBag.message = "Wrong Entries";
                        }

                    }


                }
            }
            catch (System.NullReferenceException ex)
            {
                Console.WriteLine(ex.ToString());

            }
            catch (Exception ex1)
            {
                Console.WriteLine(ex1.ToString());

            }


            return View();
            #endregion
        }

        public List<SelectListItem> GetGender()
        {
            #region Gender list
            List<SelectListItem> gender = new List<SelectListItem>()
            {
                new SelectListItem{Value="Select",Text="Select"},
                new SelectListItem{Value="M",Text="Male"},
                new SelectListItem{Value="F",Text="Female"},
                new SelectListItem{Value="O",Text="Others"},



           };
            return gender;
            #endregion
        }

        [HttpGet]
        public async Task<IActionResult> EditEmployee(int employeeId)
        {
            #region Editing/Updating Employee Get Mthod to View

            Employee employee = null;
            try
            {
                using (HttpClient client = new HttpClient())
                {
                    string endPoint = _configuration["WebApiBaseUrl"] + "Employee/GetEmployeeById?employeeId=" + employeeId;
                    //EmployeeId is apicontroleer passing argument name//api controller name and httppost name given inside httppost in Employeecontroller of api
                    using (var response = await client.GetAsync(endPoint))
                    {
                        if (response.StatusCode == System.Net.HttpStatusCode.OK)
                        {   //dynamic viewbag we can create any variable name in run time
                            var result = await response.Content.ReadAsStringAsync();
                            employee = JsonConvert.DeserializeObject<Employee>(result);
                        }
                    }
                }
            }
            catch (System.NullReferenceException ex)
            {
                Console.WriteLine(ex.ToString());

            }
            catch (Exception ex1)
            {
                Console.WriteLine(ex1.ToString());

            }
            ViewBag.genderlist = GetGender();
            return View(employee);
            #endregion
        }
        [HttpPost]
        public async Task<IActionResult> EditEmployee(Employee employee)
        {
            #region Editing Employee Post method
            ViewBag.status = "";
            try
            {
                //using grabage collection only for inbuilt classes
                using (HttpClient client = new HttpClient())
                {
                    StringContent content = new StringContent(JsonConvert.SerializeObject(employee), Encoding.UTF8, "application/json");
                    string endPoint = _configuration["WebApiBaseUrl"] + "Employee/UpdateEmployee";
                    //api controller name and its function
                    using (var response = await client.PutAsync(endPoint, content))
                    {
                        if (response.StatusCode == System.Net.HttpStatusCode.OK)
                        {   //dynamic viewbag we can create any variable name in run time
                            ViewBag.status = "Ok";
                            ViewBag.message = "Employees Details Updated Successfully!!";
                        }

                        else
                        {
                            ViewBag.status = "Error";
                            ViewBag.message = "Wrong Entries";
                        }

                    }
                }
            }
            catch (System.NullReferenceException ex)
            {
                Console.WriteLine(ex.ToString());

            }
            catch (Exception ex1)
            {
                Console.WriteLine(ex1.ToString());

            }
            return View();
            #endregion
        }

        public List<SelectListItem> ShiftTiming()
        {
            List<SelectListItem> shiftTiming = new List<SelectListItem>()
            {
                new SelectListItem { Value="Shift time", Text="Select Shift Time"},
                new SelectListItem { Value = "0", Text = "09:00 AM - 06:00 PM" },
                new SelectListItem { Value = "1", Text = "06:00 AM - 02:00 PM" },
                new SelectListItem { Value = "2", Text = "02:00 PM - 10:00 PM" },
                new SelectListItem { Value = "3", Text = "10:00 AM - 06:00 PM" },
            };
            return shiftTiming;
        }


        #region BookingSeat
        public async Task<IActionResult> BookingSeat()
        {

            List<Floor> floors = new List<Floor>();
            using (HttpClient client = new HttpClient())
            {
                string endpoint = _configuration["WebApiBaseUrl"] + "Floor/GetFloor";
                using (var response = await client.GetAsync(endpoint))
                {
                    if (response.StatusCode == System.Net.HttpStatusCode.OK)
                    {
                        var result = await response.Content.ReadAsStringAsync();
                        floors = JsonConvert.DeserializeObject<List<Floor>>(result);
                    }
                }
            }
            List<SelectListItem> floor = new List<SelectListItem>();

            //fetching the departments and adding to the Viewbag for selecting appointment
            floor.Add(new SelectListItem { Value = null, Text = "Select Floor" });
            foreach (var item in floors)
            {
                floor.Add(new SelectListItem { Value = item.FloorId.ToString(), Text = item.FloorName });
            }

            ViewBag.FloorList = floor;
            ViewBag.shiftTimings = ShiftTiming();
            return View();
        }

        
        [HttpPost]
        public async Task<IActionResult> BookingSeat(BookingSeat bookingSeat)
        {
            
            bookingSeat.EmployeeID = Convert.ToInt32(TempData["EmployeeID"]);
            TempData.Keep();
            bookingSeat.SeatId = 0;
            TempData["floorId"] = bookingSeat.Seat.FloorId;
            int bookingSeatId = 0;
            ViewBag.status = "";
            using (HttpClient client = new HttpClient())
            {
                StringContent content = new StringContent(JsonConvert.SerializeObject(bookingSeat), Encoding.UTF8, "application/json");
                string endPoint = _configuration["WebApiBaseUrl"] + "BookingSeat/AddSeatBooking";
                using (var response = await client.PostAsync(endPoint, content))
                {
                    if (response.StatusCode == System.Net.HttpStatusCode.OK)
                    {
                        var result = await response.Content.ReadAsStringAsync();
                        bookingSeatId = JsonConvert.DeserializeObject<int>(result);
                        TempData["bookingSeatId"] = bookingSeatId;
                        TempData.Keep();
                        ViewBag.status = "Ok";
                        ViewBag.message = "Booking Seat Added!!";

                    }
                    else
                    {
                        ViewBag.status = "Error";
                        ViewBag.message = "Wrong Entries";
                    }
                }
            }
            bookingSeat1 = bookingSeat;
            return RedirectToAction("GetSeatsByFloorId");
        }
        [HttpGet]
        public async Task<IActionResult> GetSeatsByFloorId()
        {
            int floorId =Convert.ToInt32(TempData["floorId"]);
            List<Seat>seats = new List<Seat>();

            using (HttpClient client = new HttpClient())
            {
                string endPoint = _configuration["WebApiBaseUrl"] + "Seat/GetAllSeatsByFloorId?floorId=" + floorId;
                //EmployeeId is apicontroleer passing argument name//api controller name and httppost name given inside httppost in Employeecontroller of api
                using (var response = await client.GetAsync(endPoint))
                {
                    if (response.StatusCode == System.Net.HttpStatusCode.OK)
                    {   //dynamic viewbag we can create any variable name in run time
                        var result = await response.Content.ReadAsStringAsync();
                        seats = JsonConvert.DeserializeObject<List<Seat>>(result);
                    }
                }
                List<SelectListItem> seats1 = new List<SelectListItem>();

                //fetching the seats and adding to the Viewbag for booking seat
                seats1.Add(new SelectListItem { Value = null, Text = "Select SeaT" });
                foreach (var item in seats)
                {
                    seats1.Add(new SelectListItem { Value = item.SeatId.ToString(), Text = item.SeatNumber });
                }

                ViewBag.seatlist = seats1;
                return View();
            }

        }
        [HttpPost]
        public async Task<IActionResult> GetSeatsByFloorId(BookingSeat bookingSeat)
        {
            BookingSeat bookingSeat1 = new BookingSeat(); 
            using (HttpClient client = new HttpClient())
            {
                string endPoint = _configuration["WebApiBaseUrl"] + "BookingSeat/GetSeatBookingById?bookingseatId=" + Convert.ToInt32(TempData["bookingSeatId"]);
                //EmployeeId is apicontroleer passing argument name//api controller name and httppost name given inside httppost in Employeecontroller of api
                using (var response = await client.GetAsync(endPoint))
                {
                    if (response.StatusCode == System.Net.HttpStatusCode.OK)
                    {   //dynamic viewbag we can create any variable name in run time
                        var result = await response.Content.ReadAsStringAsync();
                        bookingSeat1 = JsonConvert.DeserializeObject<BookingSeat>(result);
                    }
                }
            }

            bookingSeat1.SeatId = bookingSeat.SeatId;
            using (HttpClient client = new HttpClient())
            {
                StringContent content = new StringContent(JsonConvert.SerializeObject(bookingSeat1), Encoding.UTF8, "application/json");
                string endPoint = _configuration["WebApiBaseUrl"] + "BookingSeat/UpdateSeatBooking";
                using (var response = await client.PutAsync(endPoint, content))
                {
                    if (response.StatusCode == System.Net.HttpStatusCode.OK)
                    {
                        ViewBag.status = "Ok";
                        ViewBag.message = "Booking Seat Added!!";

                    }
                    else
                    {
                        ViewBag.status = "Error";
                        ViewBag.message = "Wrong Entries";
                    }
                }

                return View();
            }
        }
            



        [HttpGet]
        public async Task<IActionResult> BookingSeatHistory()
        {
            IEnumerable<BookingSeat> bookingResult = null;
            using (HttpClient client = new HttpClient())
            {

                string endPoint = _configuration["WebApiBaseUrl"] + "BookingSeat/GetAllBookingSeats";
                using (var response = await client.GetAsync(endPoint))
                {
                    if (response.StatusCode == System.Net.HttpStatusCode.OK)
                    {
                        var result = await response.Content.ReadAsStringAsync();
                        bookingResult = JsonConvert.DeserializeObject<IEnumerable<BookingSeat>>(result);
                    }
                }
            }
            return View(bookingResult);

        }


        public async Task<IActionResult> CancelBooking(int BookSeatId)
        {
            BookingSeat seat = new BookingSeat();
            using (HttpClient client = new HttpClient())
            {
                string endPoint = _configuration["WebApiBaseUrl"] + "BookingSeat/GetSeatBookingById?bookingseatId=" + BookSeatId;

                using (var response = await client.GetAsync(endPoint))
                {
                    if (response.StatusCode == System.Net.HttpStatusCode.OK)
                    {
                        var result = await response.Content.ReadAsStringAsync();
                        seat = JsonConvert.DeserializeObject<BookingSeat>(result);
                    }

                }
            }
            return View(seat);
        }

        [HttpPost]
        public async Task<IActionResult> CancelBooking(BookingSeat bookseat)

        {
            using (HttpClient client = new HttpClient())
            {
                string endPoint = _configuration["WebApiBaseUrl"] + "BookingSeat/DeleteSeatBooking?bookingseatId=" + bookseat.BookingSeatId;
                using (var response = await client.DeleteAsync(endPoint))
                {
                    if (response.StatusCode == System.Net.HttpStatusCode.OK)
                    {
                        ViewBag.status = "Ok";
                        ViewBag.message = "Booked Seat cancelled successfully";
                    }
                    else
                    {
                        ViewBag.status = "Error";
                        ViewBag.message = "Wrong Entries";
                    }
                }
            }
            return View();
        }


        public async Task<IActionResult> UpdateBooking(int BookSeatId)
        {
            BookingSeat seat = null;
            using (HttpClient client = new HttpClient())
            {
                string endPoint = _configuration["WebApiBaseUrl"] + "BookingSeat/GetSeatBookingById?bookingseatId=" + BookSeatId;

                using (var response = await client.GetAsync(endPoint))
                {
                    if (response.StatusCode == System.Net.HttpStatusCode.OK)
                    {
                        var result = await response.Content.ReadAsStringAsync();
                        seat = JsonConvert.DeserializeObject<BookingSeat>(result);
                    }

                }
            }
            return View(seat);
        }
        [HttpPost]
        public async Task<IActionResult> UpdateBooking(BookingSeat bookseat)

        {
            using (HttpClient client = new HttpClient())
            {
                StringContent content = new StringContent(JsonConvert.SerializeObject(bookseat), Encoding.UTF8, "application/json");
                string endPoint = _configuration["WebApiBaseUrl"] + "BookingSeat/UpdateSeatBooking";
                using (var response = await client.PutAsync(endPoint, content))
                {
                    if (response.StatusCode == System.Net.HttpStatusCode.OK)
                    {
                        ViewBag.status = "Ok";
                        ViewBag.message = "BookingSeat Updated!!";
                    }
                    else
                    {
                        ViewBag.status = "Error";
                        ViewBag.message = "Wrong Entries";
                    }
                }
            }
            return View();
        }

        #endregion BookingSeat

        public List<SelectListItem> MeetingHours()
        {
            List<SelectListItem> meetingHours = new List<SelectListItem>()
            {
                new SelectListItem { Value="Shift time", Text="Select Meeting Hours"},
                new SelectListItem { Value = "0", Text = "1 hour" },
                new SelectListItem { Value = "1", Text = "2 hour" },
                new SelectListItem { Value = "2", Text = "3 hour" },
                new SelectListItem { Value = "3", Text = "4 hour" },
            };
            return meetingHours;
        }



        #region BookingRoom
        public async Task<IActionResult> BookingRooms()
        {
            List<Floor> floors = new List<Floor>();
            using (HttpClient client = new HttpClient())
            {
                string endpoint = _configuration["WebApiBaseUrl"] + "Floor/GetFloor";
                using (var response = await client.GetAsync(endpoint))
                {
                    if (response.StatusCode == System.Net.HttpStatusCode.OK)
                    {
                        var result = await response.Content.ReadAsStringAsync();
                        floors = JsonConvert.DeserializeObject<List<Floor>>(result);
                    }
                }
            }
            List<SelectListItem> floor = new List<SelectListItem>();

            //fetching the departments and adding to the Viewbag for selecting appointment
            floor.Add(new SelectListItem { Value = null, Text = "Select Floor" });
            foreach (var item in floors)
            {
                floor.Add(new SelectListItem { Value = item.FloorId.ToString(), Text = item.FloorName });
            }

            ViewBag.FloorList = floor;
            ViewBag.meetingHours = MeetingHours();
            return View();
        }


        [HttpPost]
        public async Task<IActionResult> BookingRooms(BookingRoom bookingRoom)
        {
            ViewBag.status = "";
            using (HttpClient client = new HttpClient())
            {
                StringContent content = new StringContent(JsonConvert.SerializeObject(bookingRoom), Encoding.UTF8, "application/json");
                string endPoint = _configuration["WebApiBaseUrl"] + "BookingRoom/AddBookingRoom";
                using (var response = await client.PostAsync(endPoint, content))
                {
                    if (response.StatusCode == System.Net.HttpStatusCode.OK)
                    {
                        ViewBag.status = "Ok";
                        ViewBag.message = "BookingRoom details saved sucessfully!!";
                    }
                    else
                    {
                        ViewBag.status = "Error";
                        ViewBag.message = "Wrong entries!";
                    }
                }
            }
            return View();
        }


        [HttpGet]
        public async Task<IActionResult> BookingRoomHistory()
        {
            IEnumerable<BookingRoom> bookingroomresult = null;
            using (HttpClient client = new HttpClient())
            {
                string endPoint = _configuration["WebApiBaseUrl"] + "BookingRoom/GetBookingRoom";
                using (var response = await client.GetAsync(endPoint))
                {
                    if (response.StatusCode == System.Net.HttpStatusCode.OK)
                    {
                        var result = await response.Content.ReadAsStringAsync();
                        bookingroomresult = JsonConvert.DeserializeObject<IEnumerable<BookingRoom>>(result);
                    }
                }

            }
            return View(bookingroomresult);
        }

        public async Task<IActionResult> UpdateBookingRooms(int RoomId)
        {
            BookingRoom bookingRoom = null;
            using (HttpClient client = new HttpClient())
            {
                string endPoint = _configuration["WebApiBaseUrl"] + "BookingRoom/GetBookingRoomById?bookingRoomId=" + RoomId;
                using (var response = await client.GetAsync(endPoint))
                {
                    if (response.StatusCode == System.Net.HttpStatusCode.OK)
                    {
                        var result = await response.Content.ReadAsStringAsync();
                        bookingRoom = JsonConvert.DeserializeObject<BookingRoom>(result);
                    }
                }
            }
            return View(bookingRoom);

        }

        [HttpPost]

        public async Task<IActionResult> UpdateBookingRooms(BookingRoom bookingRoom)
        {
            ViewBag.status = "";
            using (HttpClient client = new HttpClient())
            {
                StringContent content = new StringContent(JsonConvert.SerializeObject(bookingRoom), Encoding.UTF8, "application/json");
                string endPoint = _configuration["WebApiBaseUrl"] + "BookingRoom/UpdateBookingRoom";
                using (var response = await client.PutAsync(endPoint, content))
                {
                    if (response.StatusCode == System.Net.HttpStatusCode.OK)
                    {
                        ViewBag.status = "Ok";
                        ViewBag.message = "BookingRoom details updated sucessfully!!";
                    }
                    else
                    {
                        ViewBag.status = "Error";
                        ViewBag.message = "Wrong entries!";
                    }
                }
            }
            return View();

        }

        public IActionResult CancelBookingRooms()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> CancelBookingRooms(int RoomId)
        {
            BookingRoom bookingRoom = null;
            using (HttpClient client = new HttpClient())
            {
                string endPoint = _configuration["WebApiBaseUrl"] + "BookingRoom/DeleteBookingRoom?bookingRoomId=" + RoomId;
                using (var response = await client.DeleteAsync(endPoint))
                {
                    if (response.StatusCode == System.Net.HttpStatusCode.OK)
                    {
                        ViewBag.status = "Ok";
                        ViewBag.message = "Booked Room cancelled sucessfully!!";
                        return RedirectToAction("Index", "BookingRoom");
                    }
                    else
                    {
                        ViewBag.status = "Error";
                        ViewBag.message = "Wrong entries!";
                    }
                }
            }
            return View();
        }
        #endregion BookingRegion
        public async Task<IActionResult> SelectingFloor()
        {
            #region Selecting the floor
            List<Floor> floors = new List<Floor>();
            using (HttpClient client = new HttpClient())
            {
                string endpoint = _configuration["WebApiBaseUrl"] + "Floor/GetFloor";
                using (var response = await client.GetAsync(endpoint))
                {
                    if (response.StatusCode == System.Net.HttpStatusCode.OK)
                    {
                        var result = await response.Content.ReadAsStringAsync();
                        floors = JsonConvert.DeserializeObject<List<Floor>>(result);
                    }
                }
            }
            List<SelectListItem> floor = new List<SelectListItem>();

            //fetching the departments and adding to the Viewbag for selecting appointment
            floor.Add(new SelectListItem { Value = null, Text = "Select Floor" });
            foreach (var item in floors)
            {
                floor.Add(new SelectListItem { Value = item.FloorId.ToString(), Text = item.FloorName });
            }

            ViewBag.FloorList = floor;
            return View();
            #endregion
        }

    }
}