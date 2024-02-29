using System;
using MySql.Data.MySqlClient;

class Program
{
    static void Main()
    {
        string connectionString = "Server=localhost;Database=evnull;Uid=root;Pwd=N@v@neet2003007;";

        using (MySqlConnection connection = new MySqlConnection(connectionString))
        {
            connection.Open();

            Console.WriteLine("Welcome to the E-Voting System!");

            while (true)
            {
                Console.WriteLine("\n1. View Candidates");
                Console.WriteLine("2. Vote");
                Console.WriteLine("3. Display Winning Candidate");
                Console.WriteLine("4. Exit");
                Console.Write("Choose an option: ");

                string option = Console.ReadLine();

                switch (option)
                {
                    case "1":
                        ViewCandidates(connection);
                        break;
                    case "2":
                        Vote(connection);
                        break;
                    case "3":
                        DisplayWinningCandidate(connection);
                        break;
                    case "4":
                        Console.WriteLine("Exiting the E-Voting System. Goodbye!");
                        return;
                    default:
                        Console.WriteLine("Invalid option. Please choose a valid option.");
                        break;
                }
            }
        }
    }

    static void ViewCandidates(MySqlConnection connection)
    {
        Console.WriteLine("\nList of Candidates:");

        using (MySqlCommand command = new MySqlCommand("SELECT * FROM Candidates", connection))
        using (MySqlDataReader reader = command.ExecuteReader())
        {
            while (reader.Read())
            {
                Console.WriteLine($"ID: {reader["CandidateID"]}, Name: {reader["CandidateName"]}");
            }
        }
    }

    static void Vote(MySqlConnection connection)
    {
        Console.Write("\nEnter the Candidate ID you want to vote for: ");
        string candidateId = Console.ReadLine();

        using (MySqlCommand command = new MySqlCommand("INSERT INTO Votes (CandidateID) VALUES (@CandidateID)", connection))
        {
            command.Parameters.AddWithValue("@CandidateID", candidateId);

            int rowsAffected = command.ExecuteNonQuery();

            if (rowsAffected > 0)
            {
                Console.WriteLine("Vote submitted successfully!");
            }
            else
            {
                Console.WriteLine("Failed to submit the vote. Please try again.");
            }
        }
    }

    static void DisplayWinningCandidate(MySqlConnection connection)
    {
        string winningCandidateSql = @"
            SELECT c.CandidateName, COUNT(v.VoteID) AS VoteCount
            FROM Candidates c
            LEFT JOIN Votes v ON c.CandidateID = v.CandidateID
            GROUP BY c.CandidateName
            ORDER BY VoteCount DESC
            LIMIT 1;
        ";

        using (MySqlCommand command = new MySqlCommand(winningCandidateSql, connection))
        using (MySqlDataReader reader = command.ExecuteReader())
        {
            if (reader.Read())
            {
                string winningCandidateName = reader["CandidateName"].ToString();
                int voteCount = Convert.ToInt32(reader["VoteCount"]);

                Console.WriteLine($"\nWinning Candidate: {winningCandidateName} with {voteCount} votes.");
            }
            else
            {
                Console.WriteLine("\nNo votes recorded yet. No winning candidate.");
            }
        }
    }
}
