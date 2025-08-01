﻿using hotelOOP;
using System.ComponentModel.Design;
using System.Security.Cryptography.X509Certificates;
using System.Transactions;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace hotelOOP
{

     public class Room
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
            this.isAvailable = isAvailable;

        }

        public override string ToString()
        {
            string availability = isAvailable ? "Available" : "Reserved";
            return $"Room Number: {roomNumber}, Type: {roomType}, Price: {price}, Status: {availability}";
        }
        public string ToFileString()
        {
            return $"{roomNumber}|{roomType}|{price}|{isAvailable}";
        }

        public static Room FromFileString(string fileString)
        {
            string[] parts = fileString.Split('|');
            if (parts.Length != 4) throw new FormatException("Invalid room data format.");
            return new Room(int.Parse(parts[0]), parts[1], double.Parse(parts[2]), bool.Parse(parts[3]));
        }

    }

    public class Reservation
    {
        public string GstName { get; set; }
        public Room ReserverRoom { get; set; }
        public int Nights { get; set; }
        public double TotalCost { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }

        public Reservation(string gstName, Room room, DateTime startDate, int nights)
        {
            GstName = gstName;
            ReserverRoom = room;
            StartDate = startDate;
            Nights = nights;
            EndDate = startDate.AddDays(nights);
            TotalCost = room.price * nights;
        }

        public override string ToString()
        {
            return $"Reservation for {GstName} from {StartDate.ToShortDateString()} to {EndDate.ToShortDateString()} - " +
                   $"Room {ReserverRoom.roomNumber}, Type: {ReserverRoom.roomType}, Nights: {Nights}, Total: OMR {TotalCost}";
        }

        public string ToFileString()
        {
            return $"{GstName}|{ReserverRoom.roomNumber}|{ReserverRoom.roomType}|{ReserverRoom.price}|{Nights}|{StartDate}|{EndDate}";
        }

        public static Reservation FromFileString(string line)
        {
            var parts = line.Split('|');
            if (parts.Length != 6)
                throw new FormatException("Invalid reservation line format.");

            string guestName = parts[0];
            int roomNumber = int.Parse(parts[1]);
            string roomType = parts[2];
            double price = double.Parse(parts[3]);
            int nights = int.Parse(parts[4]);
            DateTime startDate = DateTime.Parse(parts[5]);

            // Try to get existing room from roomList
            Room room = Program.roomList.Find(r => r.roomNumber == roomNumber);
            if (room == null)
            {
                room = new Room(roomNumber, roomType, price, false);
                Program.roomList.Add(room); // Optional: avoid duplicate room creation later
            }

            return new Reservation(guestName, room, startDate, nights);
        }

    }



}


    internal class Program
    {
        public static string reservationFilePath = "reservations.txt";
        public static string roomFilePath = "rooms.txt";
        public static List<Room> roomList = new List<Room>();
        public static List<Reservation> reservationList = new List<Reservation>();
        static void Main(string[] args)
        {
            // Load existing reservations from file
            LoadReservationsFromFile();
           // LoadRoomsFromFile();
           LoadData(); // Load rooms and reservations from files
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
                        Console.ReadKey();
                        break;
                }

                if (choice == "3")
                {
                    SaveReservationsToFile(); // Save on exit
                    Console.WriteLine("Exiting the system. Goodbye!");

                    return;
                }
            }


            static void AdminMenu()
            {
                while (true)
                {
                    Console.Clear();
                    Console.WriteLine("Admin Menu:");
                    Console.WriteLine("1. Add Room");
                    Console.WriteLine("2. View Rooms");
                    Console.WriteLine("3. View Reservations");
                    Console.WriteLine("4. Search Reservation by Guest Name");
                    Console.WriteLine("5 .Show Highest Paying Guest");
                    Console.WriteLine("6. CancelReservation");
                    Console.WriteLine("7. Exit");
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
                        case "6":
                            CancelReservation();
                            break;
                        case "7":
                            SaveReservationsToFile(); // Save before exiting
                            Console.WriteLine("Exiting Admin Menu. Goodbye!");
                            return;
                        default:
                            Console.WriteLine("Invalid choice, please try again.");
                            Console.ReadKey();
                            break;
                    }
                    Console.WriteLine("Press any key to return to the Admin Menu...");
                    Console.ReadKey();
                }
            }
            SaveData(); // Save data to files at the end of the program

            static void UserMenu()
            {
                while (true)
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

                    Console.WriteLine("\nPress any key to return to the User Menu...");
                    Console.ReadKey(); //This line was missing
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
                if (!int.TryParse(Console.ReadLine(), out int roomNumber) || roomNumber <= 0)
                {
                    Console.WriteLine("Invalid room number.");
                    return;
                }
                if (roomList.Any(r => r.roomNumber == roomNumber))
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
                SaveData(); // Save room data to file
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

            Console.Write("Enter reservation start date (yyyy-MM-dd): ");
            if (!DateTime.TryParse(Console.ReadLine(), out DateTime startDate))
            {
                Console.WriteLine("Invalid date.");
                return;
            }

            Console.Write("Enter number of nights: ");
            if (!int.TryParse(Console.ReadLine(), out int nights) || nights <= 0)
            {
                Console.WriteLine("Invalid number of nights.");
                return;
            }

            // Show available rooms
            Console.WriteLine("\nAvailable Rooms:");
            foreach (var room in roomList)
            {
                if (IsRoomAvailable(room, startDate, nights))
                {
                    Console.WriteLine(room);
                }
            }

            Console.Write("Enter Room Number to reserve: ");
            if (!int.TryParse(Console.ReadLine(), out int roomNumber))
            {
                Console.WriteLine("Invalid room number.");
                return;
            }

            Room selectedRoom = roomList.Find(r => r.roomNumber == roomNumber);
            if (selectedRoom == null || !IsRoomAvailable(selectedRoom, startDate, nights))
            {
                Console.WriteLine("Room not available or does not exist.");
                return;
            }

            Reservation newReservation = new Reservation(guestName, selectedRoom, startDate, nights);
            reservationList.Add(newReservation);
            SaveData();
            Console.WriteLine($"Reservation successful for {guestName}. Total cost: OMR {newReservation.TotalCost}");
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
            if (Program.reservationList.Count == 0)
            {
                Console.WriteLine("No reservations found.");
                return;
            }

            Reservation highest = Program.reservationList[0];

            foreach (var res in Program.reservationList)
            {
                if (res.TotalCost > highest.TotalCost)
                {
                    highest = res;
                }
            }

            Console.WriteLine("\nHighest-Paying Guest:");
            Console.WriteLine($"Guest: {highest.GstName}");
            Console.WriteLine($"Room Number: {highest.ReserverRoom.roomNumber}");
            Console.WriteLine($"Price Paid: OMR {highest.TotalCost}");
            Console.WriteLine($"Date: {highest.StartDate.ToShortDateString()}");
        }


        //Cancel a reservation by room number
        static void CancelReservation()
            {
                Console.Write("Enter room number to cancel: ");
                if (!int.TryParse(Console.ReadLine(), out int roomNumber)) return;

                var res = reservationList.Find(r => r.ReserverRoom.roomNumber == roomNumber);
                if (res != null)
                {
                    res.ReserverRoom.isAvailable = true;
                    reservationList.Remove(res);
                    Console.WriteLine("Reservation cancelled.");
                }
                else
                {
                    Console.WriteLine("No reservation found for that room.");
                }
            }



            //save reservations to a file
            static void SaveReservationsToFile()
            {
                using (StreamWriter writer = new StreamWriter(reservationFilePath))
                {
                    foreach (var res in reservationList)
                    {
                    writer.WriteLine($"{res.GstName}|{res.ReserverRoom.roomNumber}|{res.ReserverRoom.roomType}|{res.ReserverRoom.price}|{res.Nights}|{res.StartDate:yyyy-MM-dd}");
                }
                }
            }
            static void SaveData()
            {
                File.WriteAllLines("rooms.txt", roomList.Select(r => r.ToFileString()));
                File.WriteAllLines("reservations.txt", reservationList.Select(res => res.ToFileString()));
            }
      


        static bool IsRoomAvailable(Room room, DateTime startDate, int nights)
        {
            DateTime endDate = startDate.AddDays(nights);
            foreach (var reservation in reservationList)
            {
                if (reservation.ReserverRoom.roomNumber == room.roomNumber)
                {
                    if (!(endDate <= reservation.StartDate || startDate >= reservation.EndDate))
                    {
                        return false; // Date range overlaps
                    }
                }
            }
            return true;
        }


    }

    static void LoadData() // Declared as static method of the Program class
        {
            if (File.Exists("rooms.txt"))
            {
                foreach (var line in File.ReadAllLines("rooms.txt"))
                {
                    roomList.Add(Room.FromFileString(line));
                }
            }

            if (File.Exists("reservations.txt"))
            {
                foreach (var line in File.ReadAllLines("reservations.txt"))
                {
                    reservationList.Add(Reservation.FromFileString(line));
                }
            }
        }
    // Load reservations from a file
    static void LoadReservationsFromFile()
    {
        if (File.Exists(reservationFilePath))
        {
            foreach (var line in File.ReadAllLines(reservationFilePath))
            {
                Reservation res = Reservation.FromFileString(line);
                reservationList.Add(res);
            }
        }
    }
}









