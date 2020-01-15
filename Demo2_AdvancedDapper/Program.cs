using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dapper;
using HelperLibrary.Models;
using static HelperLibrary.Tools;

namespace Demo2_AdvancedDapper
{
    class Program
    {
        static void Main(string[] args)
        {
            //MapMultipleObjects();
            //MapMultipleObjectsWithParameters("Corey");
            //MultipleSets();
            //MultipleSetsWithParameters("Smith", "1212");
            //OutputParameters("Peter", "Parker");
            //RunWithTransaction("Mr", "Nobody");
            //InsertDataSet();
            Console.ReadLine();
        }

        public static void MapMultipleObjects()
        {
            using (IDbConnection cnn = new SqlConnection(GetConnectionString()))
            {
                string sql = @"select pe.*, ph.* 
                               from dbo.Person pe
                               left join dbo.Phone ph
                                 on pe.CellPhoneId = ph.Id;";

                var people = cnn.Query<FullPersonModel, PhoneModel, FullPersonModel>(sql,
                    (person, phone) => { person.CellPhone = phone; return person; });

                foreach (var p in people)
                {
                    Console.WriteLine($"{ p.FirstName } { p.LastName }: Cell: { p.CellPhone?.PhoneNumber }");
                }
            }
        }

        public static void MapMultipleObjectsWithParameters(string lastName)
        {
            using (IDbConnection cnn = new SqlConnection(GetConnectionString()))
            {
                var p = new
                {
                    LastName = lastName
                };

                string sql = @"select pe.*, ph.* from dbo.Person pe
                               left join dbo.Phone ph
                                 on pe.CellPhoneId = ph.Id
                               where pe.LastName = @LastName;";

                var people = cnn.Query<FullPersonModel, PhoneModel, FullPersonModel>(sql,
                    (person, phone) => { person.CellPhone = phone; return person; }, p);

                foreach (var person in people)
                {
                    Console.WriteLine($"{ person.FirstName } { person.LastName }: " +
                        $"Cell: { person.CellPhone?.PhoneNumber }");
                }
            }
        }

        public static void MultipleSets()
        {
            using (IDbConnection cnn = new SqlConnection(GetConnectionString()))
            {
                string sql = @"select * from dbo.Person;
                               select * from dbo.Phone;";
                List<PersonModel> people = null;
                List<PhoneModel> phones = null;

                using (var lists = cnn.QueryMultiple(sql))
                {
                    people = lists.Read<PersonModel>().ToList();
                    phones = lists.Read<PhoneModel>().ToList();
                }

                foreach (var person in people)
                {
                    Console.WriteLine($"Person: { person.FirstName } { person.LastName }");
                }

                foreach (var phone in phones)
                {
                    Console.WriteLine($"Phone Number: { phone.PhoneNumber }");
                }
            }
        }

        public static void MultipleSetsWithParameters(string lastName, string partialPhoneNumber)
        {
            using (IDbConnection cnn = new SqlConnection(GetConnectionString()))
            {
                string sql = @"select * from dbo.Person where LastName = @LastName;
                               select * from dbo.Phone where PhoneNumber like '%' + @PartialPhoneNumber + '%';";
                List<PersonModel> people = null;
                List<PhoneModel> phones = null;

                var p = new
                {
                    LastName = lastName,
                    PartialPhoneNumber = partialPhoneNumber
                };

                using (var lists = cnn.QueryMultiple(sql, p))
                {
                    people = lists.Read<PersonModel>().ToList();
                    phones = lists.Read<PhoneModel>().ToList();
                }

                foreach (var person in people)
                {
                    Console.WriteLine($"Person: { person.FirstName } { person.LastName }");
                }

                foreach (var phone in phones)
                {
                    Console.WriteLine($"Phone Number: { phone.PhoneNumber }");
                }
            }
        }

        public static void OutputParameters(string firstName, string lastName)
        {
            using (IDbConnection cnn = new SqlConnection(GetConnectionString()))
            {
                var p = new DynamicParameters();
                p.Add("@Id", 0, DbType.Int32, ParameterDirection.Output);
                p.Add("@FirstName", firstName);
                p.Add("@LastName", lastName);

                string sql = $@"insert into dbo.Person (FirstName, LastName) 
                                values (@FirstName, @LastName);
                                select @Id = @@IDENTITY";

                cnn.Execute(sql, p);

                int newIdentity = p.Get<int>("@Id");

                Console.WriteLine($"The new Id is { newIdentity }");
            }
        }

        public static void RunWithTransaction(string firstName, string lastName)
        {
            using (IDbConnection cnn = new SqlConnection(GetConnectionString()))
            {
                var p = new DynamicParameters();
                p.Add("@FirstName", firstName);
                p.Add("@LastName", lastName);

                string sql = $@"insert into dbo.Person (FirstName, LastName) 
                                values (@FirstName, @LastName)";

                cnn.Open();
                using (var trans = cnn.BeginTransaction())
                {
                    int recordsUpdated = cnn.Execute(sql, p, trans);

                    Console.WriteLine($"Records Updated: { recordsUpdated }");

                    try
                    {
                        cnn.Execute("update dbo.Person set LastName = '1'", transaction: trans);
                        trans.Commit();
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Error: { ex.Message }");
                        trans.Rollback();
                    }
                }
            }

            Console.WriteLine();
            MapMultipleObjects();
        }

        public static void InsertDataSet()
        {
            using (IDbConnection cnn = new SqlConnection(GetConnectionString()))
            {
                var troopers = GetTroopers();
                var p = new
                {
                    people = troopers.AsTableValuedParameter("BasicUDT")
                };
                
                int recordsAffected = cnn.Execute("dbo.spPerson_InsertSet", p, commandType: CommandType.StoredProcedure);

                Console.WriteLine($"Records affected: { recordsAffected }");
                Console.WriteLine();

                MapMultipleObjects();
            }
        }

        private static DataTable GetTroopers()
        {
            var output = new DataTable();

            output.Columns.Add("FirstName", typeof(string));
            output.Columns.Add("LastName", typeof(string));

            output.Rows.Add("Trooper", "12344");
            output.Rows.Add("Trooper", "25412");
            output.Rows.Add("Trooper", "62548");
            output.Rows.Add("Trooper", "95846");
            output.Rows.Add("Trooper", "25846");
            output.Rows.Add("Trooper", "44857");
            output.Rows.Add("Trooper", "95132");
            output.Rows.Add("Trooper", "68426");
            output.Rows.Add("Trooper", "78451");

            return output;
        }
    }
}
