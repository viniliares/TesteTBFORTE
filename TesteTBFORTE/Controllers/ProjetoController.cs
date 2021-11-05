using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using TesteTBFORTE.DAL;
using TesteTBFORTE.Models;

namespace TesteTBFORTE.Controllers
{
    public class ProjetoController : Controller
    {
        private readonly IConfiguration _configuration;

        public ProjetoController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public IActionResult Index()
        {
            DispatcherProjeto dpsProjeto = new DispatcherProjeto(_configuration);
            List<ProjetoVM> projetos;

            projetos = dpsProjeto.ConsultarProjeto(new ProjetoVM { Id = 0 });

            ViewBag.Projetos = projetos;

            return View();
        }

        [HttpGet]
        public IActionResult Projeto(int id)
        {
            ProjetoVM projeto = new ProjetoVM();
            try
            {
                DispatcherProjeto dspProjeto = new DispatcherProjeto(_configuration);
                if (id == 0)
                {
                    List<ProjetoVM> projetos = dspProjeto.ConsultarProjeto(new ProjetoVM { Id = 0 });

                    if(projetos.Count > 0)
                    {
                        projeto.Id = projetos[0].Id + 1;
                        projeto.CriadoEm = projetos[0].CriadoEm;
                        projeto.CriadoPor = "Administrador";
                    }
                    else
                    {
                        projeto.Id = 1;
                        projeto.CriadoEm = DateTime.Now;
                        projeto.CriadoPor = "Administrador";
                    }
                }
                else
                {
                    projeto = dspProjeto.ConsultarProjeto(new ProjetoVM { Id = id })[0];
                }

                ViewBag.Logs = dspProjeto.ConsultarLogs(projeto);
            }
            catch (Exception ex)
            {

            }

            return View(projeto);
        }

        [HttpPost]
        public IActionResult Projeto(ProjetoVM projeto)
        {
            if(projeto.Id != 0)
            {
                DispatcherProjeto dspProjeto = new DispatcherProjeto(_configuration, "Administrador");
                List<ProjetoVM> projetos = dspProjeto.ConsultarProjeto(new ProjetoVM { Id = projeto.Id });

                if (projetos.Count > 0)
                {
                    projeto.AtualizadoEm = DateTime.Now;
                    projeto.AtualizadoPor = "Administrador";

                    dspProjeto.AtualizarProjeto(projeto);
                }
                else
                    dspProjeto.InserirProjeto(projeto);
            }

            return Redirect("/Projeto/Index");
        }
    }
}
