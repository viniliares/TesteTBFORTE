using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using TesteTBFORTE.Models;

namespace TesteTBFORTE.DAL
{
    public class DispatcherProjeto
    {
        private readonly IConfiguration _configuration;
        private string _usuario;

        public DispatcherProjeto(IConfiguration configuration) : this(configuration, string.Empty) { }

        public DispatcherProjeto(IConfiguration configuration, string usuario)
        {
            _configuration = configuration;
            _usuario = usuario;
        }

        public void InserirProjeto(ProjetoVM projeto)
        {
            SqlConnection con = new SqlConnection(_configuration["ConnectionStrings:DefaultConnection"]);
            con.Open();
            SqlTransaction tran = con.BeginTransaction();

            try
            {
                LogVM log = new LogVM
                {
                    Usuario = _usuario,
                    Data = DateTime.Now,
                    Mensagem = string.Format("Foi inserido o projeto '{0}' por {1}.", projeto.Nome, _usuario)
                };
                int idLog = InserirLog(log);

                SqlCommand cmd = new SqlCommand("INSERT INTO PROJETO(ID, NOME, DESCR, CRIADOP, CRIADOE, ATUALP, ATUALE, URLJIRA, LOG) VALUES(@ID, @NOME, @DESCR, @CRIADOP, @CRIADOE, @ATUALP, @ATUALE, @URLJIRA, @LOG)", con);

                cmd.Parameters.AddWithValue("@ID", DbType.Int32).Value = projeto.Id;
                cmd.Parameters.AddWithValue("@NOME", DbType.String).Value = projeto.Nome;
                cmd.Parameters.AddWithValue("@DESCR", DbType.String).Value = projeto.Descricao;
                cmd.Parameters.AddWithValue("@CRIADOP", DbType.String).Value = projeto.CriadoPor;
                cmd.Parameters.AddWithValue("@CRIADOE", DbType.DateTime).Value = projeto.CriadoEm;
                cmd.Parameters.AddWithValue("@ATUALP", DbType.String).Value = projeto.AtualizadoPor == null ? DBNull.Value : projeto.AtualizadoPor;
                cmd.Parameters.AddWithValue("@ATUALE", DbType.DateTime).Value = projeto.AtualizadoEm == null ? DBNull.Value : projeto.AtualizadoEm;
                cmd.Parameters.AddWithValue("@URLJIRA", DbType.String).Value = projeto.UrlJira;
                cmd.Parameters.AddWithValue("@LOG", DbType.Int32).Value = idLog;

                cmd.Transaction = tran;
                cmd.ExecuteNonQuery();

                tran.Commit();
            }
            catch (Exception e)
            {
                tran.Rollback();
                throw new Exception(e.Message);
            }
            finally
            {
                con.Close();
            }
        }

        public void AtualizarProjeto(ProjetoVM projeto)
        {
            SqlConnection con = new SqlConnection(_configuration["ConnectionStrings:DefaultConnection"]);
            con.Open();
            SqlTransaction tran = con.BeginTransaction();

            try
            {
                LogVM log = new LogVM
                {
                    Usuario = _usuario,
                    Data = DateTime.Now,
                    Mensagem = string.Format("O projeto '{0}' foi atualizado por {1}.", projeto.Nome, _usuario)
                };
                int idLog = InserirLog(log);

                SqlCommand cmd = new SqlCommand("INSERT INTO PROJETO(ID, NOME, DESCR, CRIADOP, CRIADOE, ATUALP, ATUALE, URLJIRA, LOG) VALUES(@ID, @NOME, @DESCR, @CRIADOP, @CRIADOE, @ATUALP, @ATUALE, @URLJIRA, @LOG)", con);

                cmd.Parameters.AddWithValue("@ID", DbType.Int32).Value = projeto.Id;
                cmd.Parameters.AddWithValue("@NOME", DbType.String).Value = projeto.Nome;
                cmd.Parameters.AddWithValue("@DESCR", DbType.String).Value = projeto.Descricao;
                cmd.Parameters.AddWithValue("@CRIADOP", DbType.String).Value = projeto.CriadoPor;
                cmd.Parameters.AddWithValue("@CRIADOE", DbType.DateTime).Value = projeto.CriadoEm;
                cmd.Parameters.AddWithValue("@ATUALP", DbType.String).Value = projeto.AtualizadoPor == null ? DBNull.Value : projeto.AtualizadoPor;
                cmd.Parameters.AddWithValue("@ATUALE", DbType.DateTime).Value = projeto.AtualizadoEm == null ? DBNull.Value : projeto.AtualizadoEm;
                cmd.Parameters.AddWithValue("@URLJIRA", DbType.String).Value = projeto.UrlJira;
                cmd.Parameters.AddWithValue("@LOG", DbType.Int32).Value = idLog;

                cmd.Transaction = tran;

                cmd.ExecuteNonQuery();

                tran.Commit();
            }
            catch (Exception e)
            {
                tran.Rollback();
                throw new Exception(e.Message);
            }
            finally
            {
                con.Close();
            }
        }

        public List<ProjetoVM> ConsultarProjeto(ProjetoVM projeto)
        {
            SqlConnection con = new SqlConnection(_configuration["ConnectionStrings:DefaultConnection"]);
            con.Open();
            List<ProjetoVM> projetos = new List<ProjetoVM>();

            try
            {
                SqlCommand cmd;

                if (projeto.Id != 0)
                {
                    cmd = new SqlCommand("SELECT * FROM PROJETO P WHERE P.ID = @ID AND P.LOG = (SELECT MAX(X.LOG) FROM PROJETO X WHERE X.ID = P.ID)", con);
                    cmd.Parameters.AddWithValue("@ID", projeto.Id);
                }
                else
                    cmd = new SqlCommand("SELECT * FROM PROJETO P WHERE P.LOG = (SELECT MAX(X.LOG) FROM PROJETO X WHERE X.ID = P.ID)", con);

                SqlDataAdapter adapter = new SqlDataAdapter();
                adapter.SelectCommand = cmd;

                DataTable table = new DataTable();
                adapter.Fill(table);

                foreach (DataRow dr in table.Rows)
                {
                    ProjetoVM projetoVM = new ProjetoVM();
                    projetoVM.Id = Convert.ToInt32(dr["ID"]);
                    projetoVM.Nome = dr["NOME"].ToString();
                    projetoVM.Descricao = dr["DESCR"].ToString();
                    projetoVM.CriadoPor = dr["CRIADOP"].ToString();
                    projetoVM.CriadoEm = dr["CRIADOE"].ToString() == string.Empty ? DateTime.Now : Convert.ToDateTime(dr["CRIADOE"].ToString());
                    projetoVM.AtualizadoPor = dr["ATUALP"].ToString();
                    projetoVM.AtualizadoEm = dr["ATUALE"].ToString() == string.Empty ? null : Convert.ToDateTime(dr["ATUALE"].ToString());
                    projetoVM.UrlJira = dr["URLJIRA"].ToString();
                    projetoVM.IdLog = Convert.ToInt32(dr["LOG"].ToString());
                    projetos.Add(projetoVM);
                }

                return projetos;
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
            finally
            {
                con.Close();
            }
        }

        private int InserirLog(LogVM log)
        {
            SqlConnection con = new SqlConnection(_configuration["ConnectionStrings:DefaultConnection"]);
            con.Open();
            SqlTransaction tran = con.BeginTransaction();

            try
            {
                SqlCommand cmd = new SqlCommand("INSERT INTO LOG(MENSAGEM, USUARIO, DATA) VALUES(@MENSAGEM, @USUARIO, @DATA) SELECT @@IDENTITY AS 'ID';  ", con);

                cmd.Parameters.AddWithValue("@MENSAGEM", DbType.String).Value = log.Mensagem;
                cmd.Parameters.AddWithValue("@USUARIO", DbType.String).Value = log.Usuario;
                cmd.Parameters.AddWithValue("@DATA", DbType.DateTime).Value = log.Data;

                SqlDataAdapter adapter = new SqlDataAdapter();
                cmd.Transaction = tran;
                adapter.SelectCommand = cmd;

                DataTable table = new DataTable();
                adapter.Fill(table);

                int id = 0;
                foreach (DataRow dr in table.Rows)
                {
                    id = Convert.ToInt32(dr["ID"]);
                }

                tran.Commit();

                return id;
            }
            catch (Exception e)
            {
                tran.Rollback();
                throw new Exception(e.Message);
            }
            finally
            {
                con.Close();
            }
        }

        public List<LogVM> ConsultarLogs(ProjetoVM projeto)
        {
            SqlConnection con = new SqlConnection(_configuration["ConnectionStrings:DefaultConnection"]);
            con.Open();
            List<LogVM> logs = new List<LogVM>();

            try
            {
                SqlCommand cmd = new SqlCommand("SELECT L.* FROM LOG L INNER JOIN PROJETO P ON L.ID = P.LOG AND P.ID = @ID", con);

                cmd.Parameters.AddWithValue("@ID", DbType.Int32).Value = projeto.Id;

                SqlDataAdapter adapter = new SqlDataAdapter();
                adapter.SelectCommand = cmd;

                DataTable table = new DataTable();
                adapter.Fill(table);

                foreach (DataRow dr in table.Rows)
                {
                    LogVM log = new LogVM();
                    log.IdLog = Convert.ToInt32(dr["ID"]);
                    log.Mensagem = dr["MENSAGEM"].ToString();
                    log.Data = Convert.ToDateTime(dr["DATA"]);
                    logs.Add(log);
                }

                return logs;
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
            finally
            {
                con.Close();
            }
        }
    }

}
