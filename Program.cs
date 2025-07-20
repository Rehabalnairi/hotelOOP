using System.ComponentModel.Design;
using System.Security.Cryptography.X509Certificates;
using System.Transactions;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace hotelOOP
{

    class Room
    {
        public int roomNumber { get; set; }
        public string roomType { get; set; }
        public double price { get; set; }
        public bool isAvailable { get; set; } = true; // Default value is true
        public Room(int number, string type, double price, bool isAvailable)
        {
            this.roomNumber = number;
            this.roomType = type;
            this.price = price;
            this.isAvailable = true;

        }

        public override string ToString()
        {
            string availability = isAvailable ? "Available" : "Reserved";
            return $"Room Number: {roomNumber}, Type: {roomType}, Price: {price}, Status: {availability}";
        }



    }

    class Reservation
    {
        //List<Reservation> reservationList = new List<Reservation>();
        public string GstName { get; set; }
        public Room ReserverRoom { get; set; }
        public DateTime Date { get; set; }
         public int Nights { get; set; } // Number of nights reserved
        public double TotalCost { set; get; }
        public Reservation(string gstName, Room Room,int nights)
        {
            GstName = gstName;
            ReserverRoom = Room;
            Date = DateTime.Now;
            Nights = nights; // Initialize the number of nights reserved
            Room.isAvailable = false; // Mark room as reserved
            TotalCost =Room.price* Nights;
        }

  

        public override string ToString()
        {
            return $"Reservation for {GstName} on {Date.ToShortDateString()} - Room {ReserverRoom.roomNumber}, Type: {ReserverRoom.roomType}, " +
                   $"Rate: {ReserverRoom.price}/night, Nights: {Nights}, Total: ${TotalCost}";
        }
    
}
    internal class Program
    {
        static string reservationFilePath = "reservations.txt"; // File path for reservations
        static  List<Room> roomList = new List<Room>();
       static  List<Reservation> reservationList = new List<Reservation>();
        static void Main(string[] args)
        {
           
            //List<Room> roomList = new List<Room>();
            while (true)
            {
                Console.WriteLine("Welcome to the Hotel Reservation System!");
                Console.WriteLine("1.Admin mune");
                Console.WriteLine("2. User mune");
                Console.WriteLine("3. Exit");
                Console.Write("Choose an option: ");

                string choice = Console.ReadLine();

                switch (choice)
                {
                    case "1":
                        AdminMenu();
                        break;
                    case "2":
                        UserMenu();
                        break;
                    case "3":
                        Console.WriteLine("Exiting the system. Goodbye!");
                        return;
                    case "4":
                        return;
                    default:
                        Console.WriteLine("Invalid choice, please try again.");
                        break;
                }
            }


            static void AdminMenu()
            {
                Console.Clear();
                Console.WriteLine("Admin Menu:");
                Console.WriteLine("1. Add Room");
                Console.WriteLine("2. View Rooms");
                Console.WriteLine("3. View Reservations");
                Console.WriteLine("4. Search Reservation by Guest Name");
                Console.WriteLine("5. Cancel a reservation");
                Console.WriteLine("6. Exit");
                Console.Write("Choose an option: ");
                string Amdminchoice = Console.ReadLine();
                switch (Amdminchoice)
                {
                    case "1":
                        AddRoom();
                        break;
                    case "2":
                        ViewRooms();
                        break;
                    case "3":
                        ViewReservations();
                        break;
                    case "4":
                        SearchReservationByGuest();
                        break;
                        case "5":
                        ShowHighestPayingGuest();
                        break;
                    case"6":
                        CancelReservation();
                        break;
                    case "7":

                        return;
                    default:
                        Console.WriteLine("Invalid choice, please try again.");
                        break;
                }
            }

            static void UserMenu()
            {
                Console.Clear();
                Console.WriteLine("User Menu:");
                Console.WriteLine("1. View Rooms");
                Console.WriteLine("2. Make Reservation");
                Console.WriteLine("3. Back to main menu");
                Console.Write("Choose an option: ");
                string choice = Console.ReadLine();
                switch (choice)
                {
                    case "1":
                        ViewRooms();
                        break;
                    case "2":
                        Reservation();
                        break;
                    case "3":
                        return;
                    default:
                        Console.WriteLine("Invalid choice, please try again.");
                        break;
                }
            }

            static void AddRoom()
            {
                // This method allows the admin to add a new room to the hotel
                // Rate must be >= 100
                Console.Clear();
                Console.WriteLine("Add New Room:");
                if (roomList.Count >= 100)
                {
                    Console.WriteLine("Cannot add more than 100 rooms.");
                    return;
                }

                Console.Write("Enter Room Number: ");
                if(!int.TryParse(Console.ReadLine(), out int roomNumber) || roomNumber <= 0)
                {
                    Console.WriteLine("Invalid room number.");
                    return;
                }
                if(roomList.Any(r => r.roomNumber == roomNumber))
                {
                    Console.WriteLine("Room number already exists.");
                    return;
                }
                
                Console.Write("Enter Room Type (Single/Double/Suite): ");
                string type = Console.ReadLine();

                Console.Write("Enter Room Price: ");
                if (!double.TryParse(Console.ReadLine(), out double price) || price < 10)
                {
                    Console.WriteLine("Invalid price. Price must be at least 10.");
                    return;
                }

                //double price = double.Parse(Console.ReadLine());
                Room newRoom = new Room(roomNumber, type, price, true); // Adjusted constructor
                roomList.Add(newRoom);
                Console.WriteLine(" Room added successfully!");


            }

            // This method allows the user to view all available rooms in the hotel
            static void ViewRooms()
            {
                Console.WriteLine("Available Rooms:");
                if (roomList.Count == 0)
                {
                    Console.WriteLine("No rooms available.");
                }
                else
                {
                    foreach (var room in roomList)
                    {
                        Console.WriteLine(room);
                    }
                }
            }
            // This method allows the user to make a reservation for a room
            static void Reservation()
            {
                Console.Clear();
                Console.Write("Enter your name: ");
                string guestName = Console.ReadLine();

                Console.Write("Enter number of nights: ");
                if (!int.TryParse(Console.ReadLine(), out int nights) || nights <= 0)
                {
                    Console.WriteLine(" Invalid number of nights.");
                    return;
                }
                Console.WriteLine("\nAvailable Rooms:");
                foreach (var room in roomList)
                {
                    if (room.isAvailable)
                    {
                        Console.WriteLine(room);
                    }
                }
                Console.Write("Enter Room Number to reserve: ");
                if (!int.TryParse(Console.ReadLine(), out int roomNumber))
                {
                    Console.WriteLine(" Invalid room number.");
                    return;
                }

                Room selectedRoom = roomList.Find(r => r.roomNumber == roomNumber && r.isAvailable);

                if (selectedRoom == null)
                {
                    Console.WriteLine("Room not available or does not exist.");
                    return;
                }

                Reservation newReservation = new Reservation(guestName, selectedRoom, nights);
                 reservationList.Add(newReservation);

                Console.WriteLine($"Reservation successful for {guestName}. Total cost: ${newReservation.TotalCost}");
            }
                // This method allows the admin to view all reservations made by guests
                static void ViewReservations()
            {
                Console.WriteLine("Reservations:");
                if (reservationList.Count == 0)
                {
                    Console.WriteLine("No reservations found.");
                }
                else
                {
                    foreach (var reservation in reservationList)
                    {
                        Console.WriteLine(reservation);
                    }
                }

            }
            // This method allows the admin to search for a reservation by guest name
            static void SearchReservationByGuest()
            {
                Console.Write("Enter guest name to search: ");
                string searchName = Console.ReadLine().Trim().ToLower();

                bool found = false;
                foreach (var res in reservationList)
                {
                    if (res.GstName.ToLower().Contains(searchName))
                    {
                        Console.WriteLine(res);
                        found = true;
                    }
                }

                if (!found)
                {
                    Console.WriteLine("No reservation found for that name.");
                }
            }
            // This method shows the highest paying guest based on the room price
            static void ShowHighestPayingGuest()
            {
                if (reservationList.Count == 0)
                {
                    Console.WriteLine(" No reservations found.");
                    return;
                }

                Reservation highest = reservationList[0];

                foreach (var res in reservationList)
                {
                    if (res.ReserverRoom.price > highest.ReserverRoom.price)
                    {
                        highest = res;
                    }
                }

                Console.WriteLine("\n Highest-Paying Guest:");
                Console.WriteLine($"Guest: {highest.GstName}");
                Console.WriteLine($"Room Number: {highest.ReserverRoom.roomNumber}");
                Console.WriteLine($"Price Paid: omr{highest.ReserverRoom.price}");
                Console.WriteLine($"Date: {highest.Date}");
            }

            //Cancel a reservation by room number
            static void CancelReservation()
            {
                Console.Write("Enter room number to cancel reservation: ");
                if (!int.TryParse(Console.ReadLine(), out int roomNumber))
                {
                    Console.WriteLine("Invalid number.");
                    return;
                }
                Reservation reservationToCancel = reservationList.Find(r => r.ReserverRoom.roomNumber == roomNumber);
                if (reservationToCancel != null)
                {
                    reservationList.Remove(reservationToCancel);
                    reservationToCancel.ReserverRoom.isAvailable = true; // Mark room as available
                    Console.WriteLine("Reservation cancelled successfully.");
                }
                else
                {
                    Console.WriteLine("No reservation found for that room number.");
                }
            }

        }
    }
}


      

