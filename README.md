# Pizza Pulse Back-End API - Secure Cloud Architecture

> **🔗 Demo Online:** [Acesse a Documentação Swagger (Ao Vivo)](https://pizzaria-api-gustavo-marcialis-a2dwbpfrdxgec4bp.centralus-01.azurewebsites.net)

Este é o Back-End que se conecta com o Front-End [Pizza Pulse Frontend](https://github.com/gustavo-marcialis/Pizza-Pulse-Frontend)

![.NET 8](https://img.shields.io/badge/.NET-8.0-purple)
![Azure](https://img.shields.io/badge/Azure-Cloud-blue)
![Security](https://img.shields.io/badge/Security-SC--900-green)
![Build Status](https://img.shields.io/github/actions/workflow/status/gustavo-marcialis/sistema-de-pedidos/main.yml)

## Sobre o Projeto
Este projeto é uma API RESTful desenvolvida em **.NET 8**, focada não apenas na lógica de negócios de uma pizzaria, mas principalmente na implementação de práticas modernas de **Cloud Computing** e **Segurança da Informação**, alinhadas aos objetivos da certificação **Microsoft SC-900 (Security, Compliance, and Identity Fundamentals)**.

A aplicação simula um sistema onde clientes podem fazer pedidos (acesso público) e funcionários gerenciam esses pedidos com níveis de permissão distintos (acesso seguro).

---

## Implementações de Segurança & SC-900
Este projeto serve como prova de conceito para os pilares de segurança da Microsoft:

### 1. Identidade e Acesso (Identity & Access Management)
* **Microsoft Entra ID (Azure AD):** A autenticação não é feita no banco de dados local, mas gerenciada pelo provedor de identidade na nuvem.
* **RBAC (Role-Based Access Control):** Implementação do **Princípio do Menor Privilégio**.
    * **Role `Pizzaiolo`:** Permissão para alterar status de pedidos.
    * **Role `Garcom`:** Permissão para visualizar e anotar pedidos.
    * **Guest:** Acesso anônimo limitado apenas à criação de pedidos.
* **JWT (JSON Web Tokens):** Segurança stateless via tokens Bearer.

### 2. Proteção de Infraestrutura (Infrastructure Security)
* **Zero Trust:** A API "não confia" em ninguém por padrão. Rotas sensíveis exigem autenticação explícita (`[Authorize]`).
* **Segurança de Segredos:** As Connection Strings de produção **não estão no código** (GitHub). Elas são injetadas via **Variáveis de Ambiente** no Azure App Service, mantendo o `appsettings.json` limpo.
* **HTTPS:** Todo tráfego é forçado via SSL/TLS.

### 3. Governança e Conformidade (Governance)
* **Resource Locks:** Implementação de bloqueios de exclusão (`CanNotDelete`) no Grupo de Recursos do Azure para prevenir erros humanos e garantir a disponibilidade do serviço.
* **Documentação Viva:** Swagger UI configurado com suporte a autenticação JWT para testes de penetração e auditoria de endpoints.

---

## ☁️ Arquitetura e DevOps
O projeto utiliza uma esteira de CI/CD moderna:

* **Cloud Provider:** Microsoft Azure (Region: Central US).
* **Compute:** Azure App Service (PaaS) rodando em Linux/Windows.
* **Database:**
    * *Dev:* LocalDB (SQL Express).
    * *Prod:* Azure SQL Database (preparado para conexão).
* **CI/CD:** **GitHub Actions**. Qualquer commit na branch `main` dispara um workflow que compila o código .NET 8 e faz o deploy automático para a nuvem.

---

## Tecnologias Utilizadas
* **C# / .NET 8 (LTS)**
* **Entity Framework Core** (ORM)
* **Microsoft.Identity.Web** (Integração Entra ID)
* **Swagger / OpenAPI** (Documentação)
* **Azure Portal & CLI**

---

## 🚀 Como Rodar Localmente

### Pré-requisitos
* SDK .NET 8.0
* SQL Server (LocalDB)

### Passos
1.  Clone o repositório:
    ```bash
    git clone [https://github.com/gustavo-marcialis/sistema-de-pedidos.git](https://github.com/gustavo-marcialis/sistema-de-pedidos.git)
    ```
2.  Configure a string de conexão no `appsettings.json` (apontando para seu LocalDB).
3.  Execute as migrations:
    ```bash
    dotnet ef database update
    ```
4.  Rode a API:
    ```bash
    dotnet run
    ```

---

## Endpoints Principais

| Método | Rota | Permissão | Descrição |
| :--- | :--- | :--- | :--- |
| `POST` | `/api/Cliente/pedidosCliente` | **Pública** | Cliente faz um novo pedido. |
| `GET` | `/api/Cliente/pedidosCliente/{Mesa}` | **Pública** | Cliente consulta status do pedido. |
| `GET` | `/api/API/pedidos` | 🔐 **Pizzaiolo/Garçom** | Lista todos os pedidos. |
| `PUT` | `/api/API/alterarStatus/{id}` | 🔐 **Pizzaiolo** | Atualiza o status (ex: "No Forno"). |

---

## Roadmap & Melhorias Futuras

### Segurança Avançada (SC-900)
Este projeto utiliza o tier gratuito do Azure (Free Tier). Em um ambiente de produção empresarial com licenças **Microsoft Entra ID P1/P2**, as seguintes implementações seriam mandatórias:

* **MFA via Acesso Condicional:** Configurar políticas para exigir Autenticação Multifator (MFA) obrigatoriamente para usuários com as roles `Pizzaiolo` e `Admin`, enquanto usuários `Garçom` poderiam ter acesso simplificado dentro da rede corporativa (Trusted Location).
* **Identity Protection:** Monitoramento de riscos de entrada (ex: viagens impossíveis ou IP anônimo).

### Gestão de Segredos & Monitoramento
* **Azure Key Vault:** Migração das Connection Strings (atualmente em Variáveis de Ambiente) para o Azure Key Vault, implementando rotação automática de credenciais e acesso via Managed Identity.
* **SIEM & Observabilidade:** Integração com **Azure Monitor** e **Application Insights** para detecção de anomalias em tempo real e criação de alertas de segurança (ex: múltiplos erros 401).
* **Proteção de Rede:** O projeto herda a proteção DDoS Basic do Azure, mas em produção seria avaliado o uso do **Azure DDoS Protection Standard** e **Azure Front Door** (WAF) para mitigação de ataques na camada de aplicação.

