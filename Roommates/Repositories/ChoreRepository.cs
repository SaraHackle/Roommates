using System;
using System.Collections.Generic;
using Microsoft.Data.SqlClient;
using Roommates.Models;

namespace Roommates.Repositories
{
    public class ChoreRepository :BaseRepository
    {
        public ChoreRepository(string connectionString) : base(connectionString) { }

        public List<Chore> GetAll()
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();

                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = "SELECT Id, Name FROM Chore";

                    using(SqlDataReader reader = cmd.ExecuteReader())
                    {
                        List<Chore> chores = new List<Chore>();
                        
                        while (reader.Read())
                        {
                            int idColumnPosition = reader.GetOrdinal("Id");
                            int idValue = reader.GetInt32(idColumnPosition);
                            int nameColumnPosition = reader.GetOrdinal("Name");
                            string nameValue = reader.GetString(nameColumnPosition);

                            Chore chore = new Chore
                            {
                                Id = idValue,
                                Name = nameValue
                            };

                            chores.Add(chore);
                        }
                        return chores;
                    }
                }
            }
        }
        public Chore GetById (int id)
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = "SELECT Name FROM Chore WHERE Id = @id ";
                    cmd.Parameters.AddWithValue("@id", id);

                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        Chore chore = null;

                        if (reader.Read())
                        {
                            chore = new Chore
                            {
                                Id = id,
                                Name = reader.GetString(reader.GetOrdinal("Name"))
                            };
                        }
                        return chore;
                    }
                }
            }
        }
        public void Insert(Chore chore)
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"INSERT INTO Chore (Name)
                                        OUTPUT INSERTED.Id
                                        VALUES(@name)";
                    cmd.Parameters.AddWithValue("@name", chore.Name);

                    int id = (int)cmd.ExecuteScalar();

                    chore.Id = id;
                }
            }
        }
        public List<Chore> GetUnassignedChore()
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();

                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = "SELECT Name, Chore.Id FROM Chore LEFT JOIN RoommateChore ON RoommateChore.ChoreId = Chore.Id  WHERE RoommateChore.Id is NULL";

                    using (SqlDataReader reader = cmd.ExecuteReader())
                    { 

                        List<Chore> unassignedChores = new List<Chore>();

                        while (reader.Read())
                        {
                            int idColumnPosition = reader.GetOrdinal("Id");
                            int idValue = reader.GetInt32(idColumnPosition);
                            int nameColumnPosition = reader.GetOrdinal("Name");
                            string nameValue = reader.GetString(nameColumnPosition);

                            Chore unassignedChore = new Chore
                            {
                                Id = idValue,
                                Name = nameValue
                            };

                            unassignedChores.Add(unassignedChore);
                        }
                        return unassignedChores;
                    }
                }
            }
        }
         public void AssignChore( int choreId, int roommateId)
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"INSERT INTO RoommateChore (ChoreId, RoommateId) 
                                         VALUES (@ChoreId, @RoommateId)";
                    cmd.Parameters.AddWithValue("@ChoreId", choreId);
                    cmd.Parameters.AddWithValue("@RoommateId", roommateId);

                    cmd.ExecuteNonQuery();
                }
            }

            // when this method is finished we can look in the database and see the new room.
        }

        
    }
}
