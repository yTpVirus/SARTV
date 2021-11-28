using System;
using System.Data;
using System.Data.SQLite;
using System.Diagnostics;
using System.IO;

namespace Server.SQL
{
    class SQLManager
    {

        private static SQLiteConnection connection;

        public SQLManager()
        { Debug.WriteLine("SQLMANAGER INICIANDO!");}
        /// <summary>
        /// Cria um Novo Arquivo de Database
        /// </summary>
        /// <param name="name">Nome do Arquivo</param>
        public static void CreateNewDBFile(string name)
        {
            try
            {
                if (!File.Exists($@"resources\[MAIN]\MAIN\Database\{name}.db"))
                {
                    SQLiteConnection.CreateFile($@"resources\[MAIN]\MAIN\Database\{name}.db");
                    //File.Create($@"resources\[MAIN]\MAIN\Database\{name}.db");
                }
            }
            catch
            {
                throw;
            }
        }

        private static SQLiteConnection DBConnect(string name)
        {
            connection = new SQLiteConnection($@"Data Source = resources\[MAIN]\MAIN\Database\{name}.db; Version=3;");
            connection.Open();
            return connection;
        }
        /// <summary>
        /// Cria Uma Tabela Com os Valores Fornecidos
        /// </summary>
        /// <param name="dbname">Nome do Arquivo a Ser Acessado</param>
        /// <param name="name">Nome da Tabela a Ser Criada</param>
        /// <param name="Values">Valores a ser Adicionados EX: (valor TEXT, valor INT, valor REAL)</param>
        public static void CreateTable(string dbname,string name,string Values)
        {
            try
            {
                using (var cmd = DBConnect(dbname).CreateCommand())
                {
                    cmd.CommandText = $"CREATE TABLE IF NOT EXISTS {name} ({Values})";
                    cmd.ExecuteNonQuery();
                }
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }
        /// <summary>
        /// Pega Valores De Uma Tabela de acordo Com o Comando
        /// </summary>
        /// <param name="dbname">Nome do Banco de Dados</param>
        /// <param name="Command">Comando a Ser Executado EX: (SELECT * FROM table)</param>
        /// <returns>await result</returns>
        public static DataTable GetDataFromTable(string dbname, string Command)
        {
            SQLiteDataAdapter da = null;
            DataTable dt = new DataTable();
            try
            {
                using (var cmd = DBConnect(dbname).CreateCommand())
                {
                    cmd.CommandText = Command;
                    da = new SQLiteDataAdapter(cmd.CommandText, DBConnect(dbname));
                    da.Fill(dt);
                    return dt;
                }
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }
        /// <summary>
        /// Insere Valores Na Tabela Especificada, Somente JSON é Aceito
        /// </summary>
        /// <param name="dbname">Nome do Banco De Dados</param>
        /// <param name="name">Nome da Tabela</param>
        /// <param name="Columns">Coluna a Ser Editada Ex: (coluna1, coluna2, coluna3)</param>
        /// <param name="json">JSON String Para ser Guardado Na Coluna</param>
        public static void InsertDataOnTable(string dbname,string name,string Columns,string json)
        {
            try
            {
                using (var cmd = DBConnect(dbname).CreateCommand())
                {
                    cmd.CommandText = $"INSERT INTO {name} ({Columns}) VALUES('{json}')";
                    cmd.ExecuteNonQuery();
                }
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }
        
        /// <summary>
        /// Insere Valores Na Tabela Especificada, Somente JSON é Aceito
        /// </summary>
        /// <param name="dbname">Nome do Banco De Dados</param>
        /// <param name="name">Nome da Tabela</param>
        /// <param name="Columns">Coluna a Ser Editada Ex: (coluna1, coluna2, coluna3)</param>
        /// <param name="v">Tabela de String com valores a serem alocados</param>
        public static void InsertRaceDataOnTable(string dbname,string name,string Columns,string[] v)
        {
            try
            {
                using (var cmd = DBConnect(dbname).CreateCommand())
                {
                    cmd.CommandText = $"INSERT INTO {name} ({Columns}) VALUES('{v[0]}','{v[1]}','{v[2]}')";
                    cmd.ExecuteNonQuery();
                }
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        /// <summary>
        /// Atualizar Valores na Tabela Especificada
        /// </summary>
        /// <param name="dbname">Nome do Banco de Dados</param>
        /// <param name="name">Nome da Tabela</param>
        /// <param name="values">Valores a Serem Editados EX: (city = 'Toronto', state = 'ON', postalcode = 'M5P 2N7') <= Obrigatório Usar ''</param>
        /// <param name="target">Valor Alvo EX: (account = 4) Não usa '' </param>
        public static void UpdateDataOnTable(string dbname, string name, string values, string target)
        {
            try
            {
                using (var cmd = DBConnect(dbname).CreateCommand())
                {
                    cmd.CommandText = $"UPDATE {name} SET {values} WHERE {target}";
                    cmd.ExecuteNonQuery();
                }
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        /// <summary>
        /// Inserir Valores de Jogador Na Tabela Especificada
        /// </summary>
        /// <param name="dbname">Nome do Banco de Dados</param>
        /// <param name="account">Nome da Conta do Player</param>
        /// <param name="Columns">Coluna a Ser Editada EX: (coluna1, coluna2, coluna3)</param>
        /// <param name="values">Valor a ser Salvo na(s) Coluna(s) Especificada EX('Peixe', 'Sardinha', 'Exemplo') <= Obrigatório usar ''</param>
        public static void InsertPlayerDataOnTable(string dbname, string account, string Columns, string values)
        {
            try
            {
                using (var cmd = DBConnect(dbname).CreateCommand())
                {
                    cmd.CommandText = $"INSERT INTO {account} ({Columns}) VALUES ({values})";
                    cmd.ExecuteNonQuery();
                }
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }
        /// <summary>
        /// Inserir Valores de Jogador Na Tabela Especificada
        /// </summary>
        /// <param name="dbname">Nome do Banco de Dados</param>
        /// <param name="account">Nome da Conta do Player</param>
        /// <param name="values">Valores a Serem Editados EX: (city = 'Toronto', state = 'ON', postalcode = 'M5P 2N7') <= Obrigatório Usar ''</param>
        /// <param name="target">Valor Alvo EX: (account = 4) Não Precisa de ''</param>
        public static void UpdatePlayerDataOnTable(string dbname, string account, string values, string target)
        {
            try
            {
                using (var cmd = DBConnect(dbname).CreateCommand())
                {
                    cmd.CommandText = $"UPDATE {account} SET {values} WHERE {target}";
                    cmd.ExecuteNonQuery();
                }
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }
        public static void ExecuteRawSQLCommand(string dbname,string Command)
        {
            try
            {
                using (var cmd = DBConnect(dbname).CreateCommand())
                {
                    cmd.CommandText = Command;
                    cmd.ExecuteNonQuery();
                }
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        public static int GetRowsFromTable(string dbname,string tablename)
        {
            var data = GetDataFromTable(dbname,$"SELECT * FROM {tablename}");
            int res = data.Rows.Count;
            return res;
        }
    }
}
