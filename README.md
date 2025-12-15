# Sistema de Pedidos
  
![C#](https://img.shields.io/badge/c%23-%23239120.svg?style=for-the-badge&logo=c-sharp&logoColor=white) ![.Net](https://img.shields.io/badge/.NET-5C2D91?style=for-the-badge&logo=.net&logoColor=white) ![MicrosoftSQLServer](https://img.shields.io/badge/Microsoft%20SQL%20Server-CC2927?style=for-the-badge&logo=microsoft%20sql%20server&logoColor=white)
  
  
## Problema Resolvido
Otimização do fluxo de pedidos em pizzarias com atendimento presencial. Esta API serve como o motor central para eliminar a dependência de garçons em pedidos simples, permitindo que futuras interfaces (Mobile ou Web) se conectem para registrar pedidos vinculados diretamente às mesas.

## Solução
Uma **API RESTful** robusta que centraliza as regras de negócio e a segurança dos dados:
- **Gestão de Pedidos:** Endpoints para criação, leitura, atualização e cancelamento de pedidos.
- **Segurança Centralizada:** Validação de tokens e regras de acesso (RBAC) direto no backend, independente do frontend utilizado.
- **Banco de Dados:** Persistência segura utilizando SQL Server.

## Benefícios
- **Escalabilidade:** A API está pronta para receber conexões de múltiplos frontends (App do Cliente, Painel da Cozinha, Totem).
- **Integridade:** Garante que pedidos só mudem de status se as regras de negócio forem respeitadas.
- **Auditoria:** Graças à autenticação centralizada, cada ação na cozinha é rastreável.

## Tecnologias Utilizadas
- **Linguagem/Framework:** .NET 7 (C#) Web API.
- **ORM:** Entity Framework Core.
- **Banco de Dados:** SQL Server.
- **Identidade:** Microsoft Identity Web (Integração com Entra ID).
- **Documentação:** Swagger/OpenAPI.

---

## Segurança & Identidade (SC-900)
Este projeto aplica na prática os conceitos de segurança moderna exigidos na certificação **Microsoft SC-900**:

### 1. Identidade como Perímetro
A API não gerencia usuários ou senhas localmente. Ela delega essa responsabilidade para o **Microsoft Entra ID**.
- O sistema valida Tokens JWT em cada requisição (`[Authorize]`).
- Elimina o risco de vazamento de credenciais via SQL Injection ou acesso ao banco.

### 2. Privilégio Mínimo (RBAC)
O código implementa verificação de **Roles** para garantir que cada funcionário tenha apenas o acesso necessário:
- **Role `Pizzaiolo`:** Pode apenas alterar o *status* (Em preparo -> Pronto). Tentativas de editar o pedido são bloqueadas.
- **Role `Garcom`:** Pode alterar os itens do pedido, mas é bloqueado de editar pedidos já finalizados.

### 3. Zero Trust (Confiança Zero)
A API adota a postura de "Nunca confiar, sempre verificar".
- Não existe "rede segura": mesmo requisições locais exigem autenticação.
- Validação explícita de entradas e identidades em todos os endpoints críticos.

---

## Arquitetura e Fluxo

O Backend atua como a fonte da verdade, protegendo os dados contra acessos não autorizados de qualquer origem.

### Fluxo de Autorização (Exemplo: Cozinha)
O diagrama abaixo ilustra como a API protege a operação de "Finalizar Pedido", garantindo que apenas o funcionário correto execute a ação.

```mermaid
sequenceDiagram
    participant App as Frontend (Futuro)
    participant Azure as Microsoft Entra ID
    participant API as .NET API (Este Repositório)
    participant DB as SQL Server

    Note over App, API: Fluxo OIDC (O Frontend obtém o Token)
    App->>API: PUT /api/pedidos/10 (Bearer Token)
    
    Note over API: Validação Zero Trust
    API->>API: Valida Assinatura do Token (Entra ID)
    API->>API: Verifica Claims (Role: "Pizzaiolo")
    
    alt Autorizado
        API->>DB: Atualiza Status para "Pronto"
        DB-->>API: Confirmação
        API-->>App: 200 OK
    else Não Autorizado (ex: Garçom tentando finalizar)
        API-->>App: 403 Forbidden
    end
