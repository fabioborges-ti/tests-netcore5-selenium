using DWLaw.Selenium.Tests.Dtos;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace Selenium.Tests
{
    public class UnitTest1
    {
        [Fact]
        public void Test1()
        {
            ChromeDriver driver = CarregarPagina();

            CarregarTela(driver);

            CarregarProcesso(driver);
        }

        private static ChromeDriver CarregarPagina()
        {
            var driver = new ChromeDriver();

            driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(5);

            driver.Url = "https://www.trt5.jus.br/";

            driver.FindElement(By.CssSelector("#edit-submit")).Click();

            return driver;
        }

        private static void CarregarProcesso(ChromeDriver driver)
        {
            var classe_processo = driver.FindElements(By.ClassName("classe-processo"));

            var rows = classe_processo.FirstOrDefault(c => c.Displayed).FindElements(By.XPath("div"));

            var itensResumo = new List<ResumoDto>();

            #region CAPTURA DOS ADVOGADOS

            foreach (var row in rows)
            {
                var tabela_resumo = new ResumoDto();

                var label = row.FindElements(By.TagName("label")).FirstOrDefault().Text;

                var spans = row.FindElements(By.TagName("span"));

                if (label != "Advogado(s):")
                {
                    foreach (var s in spans)
                    {
                        tabela_resumo.Nome = label;
                        tabela_resumo.Descricao = s.Text;
                    }
                }
                else
                {
                    var advogados = new List<AdvogadoDto>();

                    foreach (var s in spans)
                    {
                        tabela_resumo.Nome = label;
                        tabela_resumo.Descricao = string.Empty;

                        var aux = s.Text.IndexOf(" ");
                        var oab_completa = s.Text[..aux];

                        var oab = oab_completa.Split("-")[0];
                        var uf = oab_completa.Split("-")[1];
                        var nome_completo = s.Text[aux..];

                        var advogado = new AdvogadoDto
                        {
                            Nome = nome_completo,
                            Oab = oab,
                            Uf = uf,
                        };

                        advogados.Add(advogado);
                    }

                    tabela_resumo.Advogados = advogados;
                }

                itensResumo.Add(tabela_resumo);
            }

            #endregion

            var index_ativo = itensResumo.FindIndex(x => x.Nome == "Polo Ativo:");
            var index_passivo = itensResumo.FindIndex(x => x.Nome == "Polo Passivo:");

            #region ADVOGADOS ATIVOS

            var advogados_ativos = new List<AdvogadoDto>();

            if (itensResumo[index_ativo + 1].Advogados.Any())
            {
                for (int index = index_ativo + 1; index < index_passivo; index++)
                {
                    var advogado = new AdvogadoDto();

                    foreach (var item in itensResumo[index].Advogados)
                    {
                        advogado.Nome = item.Nome;
                        advogado.Oab = item.Oab;
                        advogado.Uf = item.Uf;

                        advogados_ativos.Add(item);
                    }
                }
            }

            #endregion

            #region ADVOGADOS PASSIVOS

            var advogados_passivos = new List<AdvogadoDto>();

            if (itensResumo[index_ativo + 1].Advogados.Any())
            {
                var advogado = new AdvogadoDto();

                foreach (var item in itensResumo[index_passivo + 1].Advogados)
                {
                    advogado.Nome = item.Nome;
                    advogado.Oab = item.Oab;
                    advogado.Uf = item.Uf;

                    advogados_passivos.Add(item);
                }
            }

            #endregion

            #region PREENCHER POLO ATIVO

            var polo_ativo = new PoloDto
            {
                Nome = itensResumo[index_ativo].Descricao,
                Classificacao = "Polo Ativo",
                Tipo = "A",
                Advogados = advogados_ativos
            };

            #endregion

            #region PREENCHER POLO PASSIVO

            var polo_passivo = new PoloDto
            {
                Nome = itensResumo[index_ativo].Descricao,
                Classificacao = "Polo Passivo",
                Tipo = "P",
                Advogados = advogados_passivos
            };

            #endregion
        }

        private static void CarregarTela(ChromeDriver driver)
        {
            var processo = "0000198-27.2021.5.05.0005";

            var cnj = processo[..7];
            var dig = processo.Substring(8, 2);
            var ano = processo.Substring(11, 4);
            var jus = processo.Substring(16, 1);
            var trt = processo.Substring(18, 2);
            var vara = processo.Substring(21, 4);

            driver.FindElement(By.CssSelector("#edit-p-seq-proc-cnj")).SendKeys(cnj);
            driver.FindElement(By.CssSelector("#edit-p-dig-verif-cnj")).SendKeys(dig);
            driver.FindElement(By.CssSelector("#edit-p-ano-proc-cnj")).SendKeys(ano);
            driver.FindElement(By.CssSelector("#edit-p-cod-justica-cnj")).SendKeys(jus);
            driver.FindElement(By.CssSelector("#edit-p-regiao-cnj")).SendKeys(trt);
            driver.FindElement(By.CssSelector("#edit-p-cod-vara-cnj")).SendKeys(vara);

            driver.FindElement(By.Id("edit-submit")).Click();
        }
    }
}
