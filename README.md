# Projeto: Lead Manager (Back-end)

O que é o Lead Manager?<br/>
É um projeto que tem como objetivo permitir gerenciar de maneira simples e intuitiva - através de operações de listagem, adiçāo, atualizaçāo e remoção - dados de leads.
A parte de front-end do projeto consiste atualmente em duas telas.
Uma para listagem de leads a partir da qual os usuários são capazes de:
- Visualizar uma lista contendo os dados principais de leads existentes de maneira paginada
- Ir para a tela de adicionar novos leads
  - Via cadastro manual
  - Via arquivos em lote no formato CSV
- Selecionar um lead a fim de removê-lo ou
- Selecionar um lead e acionar o botão para atualizar os respectivos dados<br/>
E outra tela para as operações de adicionar ou atualizar um lead previamente selecionado

O projeto está em constante evolução e utiliza a seguinte plataforma e linguagens, tecnologias, funcionalidades e ferramentas:
- .Net Core 7
- Linguagem C#
- Orientação a objetos
- Práticas de Código limpo / Clean code
- Arquitetura Limpa / Clean architecture, constituída pelas camadas: Core, Application, Infrastructure e Presentation
- Arquitetura dirigida a eventos através de eventos de domínio e eventos de integração
- Programação assíncrona baseada em Tasks
- Asp.Net Core Web Api
- Swagger
- Entity Framework Core
- Sql Server
- Sqlite
- Azurite
- Redis Cache
- RabbitMQ
- Serialização binária como Protobuf para performance e economia de espaço (compõe as features que fazem uso de cache)
- Testes unitários / Unit tests / TDD com xUnit e Fluent Assertions
- Testes de integração / Integration tests com WebApplicationFactory e MockHttp
- Integração com o serviço de localização de endereços ViaCep via HttpClient tipado
- Aplicação de princípios SOLID
- Aplicação de padrões de projeto / design patterns como: Object mother, Singleton, Mediator, Factory, Factory method, Retry
- Aplicação do padrão arquitetural CQRS de maneira lógica com MediatR
- Uso de Pipeline Behaviors para validação de dados de entrada em combinação com FluentValidations
- Uso de Design dirigido a domínio / domain-driven design / DDD para modalagem das entidades e dos comportamentos da classes e objetos de valor
- Integração com Azurite para armazenamento de arquivos
- Gravação de log com múltiplos 'Sinks' com Serilog (Console, Arquivo e banco de dados)
- Integração com Redis para armazenamento de dados em cache para ganho de performance em geral
- Integração com RabbitMQ para mensageria de dados, ajudando na composição da implementação de arquittura dirigida a eventos / event-driven architecture / EDA
- EM BREVE: Confirmação de remoção de lead mediante informação de token recebido (fake) via SMS/ WhatsApp

Pré-requisitos para execução do 'back-end' da aplicação<br/>
É necessário possuir os seguintes componentes instalados na máquina:
- SDK do .Net Core 7 (que pode ser obtido através da url: https://nodejs.org/en](https://dotnet.microsoft.com/en-us/download/dotnet/7.0)
- Docker<br/>
  Caso a máquina seja Mac, siga os passos conforme a url: https://docs.docker.com/desktop/install/mac-install/<br/>
  Caso a máquina seja Linux, siga os passos conforme a url: https://docs.docker.com/desktop/install/linux-install/#generic-installation-steps<br/>
  Caso a máquina seja Windows, siga os passos conforme a url: https://docs.docker.com/desktop/install/windows-install/<br/>
- Azurite<br/>
  Siga os passos conforme a url https://learn.microsoft.com/en-us/azure/storage/common/storage-use-azurite?tabs=visual-studio<br/>

Como executar o projeto localmente?
- Garanta que a máquina esteja devidamente configurada, conforme a seção "Pré-requisitos para execução do Front-End da aplicação"
- Execute o Docker
- Acesse o Terminal, Command Prompt ou Powershell
- Execute o seguinte comando para subir um servidor de banco de dados Sql Server:<br/>
  docker run -e "MSSQL_PID=Express" -e "ACCEPT_EULA=Y" -e "SA_PASSWORD=Y0urStr0nGP@sswoRD_2023" -p 1433:1433 --name leadmanager-db -d mcr.microsoft.com/mssql/server<br/>
- Execute o seguinte comando para subit um servidor de caching Redis:<br/>
  docker run --name leadmanager-redis -p 6379:6379 -d redis:7.0.12-alpine<br/>
  OU<br/>
  docker run --name leadmanager-redis -p 6379:6379 -d redis/redis-stack-server:latest
- Execute o seguinte comando para subir um servidor de mensageria RabbitMQ:<br/>
  docker run -d --hostname leadmanager-rabbit-management --name leadmanager-messagebus -p 8080:15672 -p 5672:5672 rabbitmq:3-management
- Acesse outro Terminal, Command Prompt ou Powershell
- Execute o Azurite através do seguinte comando:<br/>
  azurite-blob -l X:\Path\to\blobs
- Navegue até a pasta raíz do projeto (mesma pasta que contém o arquivo LeadManager.sln, por exemplo)
- Execute os seguintes comandos:<br/>
  dotnet build<br/>
  dotnet run<br/>
  (O comando irá compilar a solução, subirá um servidor Kestrel e automaticamente abrirá o navegador web padrão apontado para o Swagger da API)<br/>
NOTA: Em breve, essas etapas para execução da aplicação na máquina local serão simplificadas a uma única linha de comando através do uso de Docker-Compose.<br/>

Backlog:
- (Technical debt) Criar Dockerfile do projeto
- (Technical debt) Criar Docker-compose no projeto
- (Technical debt) Adicionar a aplicação ao Docker-Compose para simplificar a configuração da máquina e permitir automatizar a execução da mesma em uma única linha de comando
- (Technical debt) Proteger a API contra acesso indevido, de maneira que somente usuários autenticados possam invocar os endpoints
  - Possibilidade 1: a aplicação deverá ser capaz de encaminhar a solicitação de autenticação para um servidor de identidade a fim de obter o Token de autenticação
  - Possibilidade 2: a aplicação deverá ser capaz de validar tokens de autenticação/autorização - incluindo Claims - que possibilitem ou recusem executar os endpoints da API
- (Technical debt) Implementar um filtro de ação que envia um SMS / mensagem no WhatsApp (de maneira Fake) ao usuário contendo um token de 4 dígitos numéricos para realizar a operação de Remoção de Leads
  - A ideia: na solicitação de remoção, deverá ser informado no cabeçalho HTTP um header X-Confirmation-Number com um código válido.
    - Se na solicitação não veio este cabeçalho, a aplicação deverá gerar e armazenar um token com tempo de expiração de 1 minuto.
    - Se na solicitação veio o cabeçalho, a aplicação deverá validar se o token ainda é válido (informou o token certo + token não está expirado), podendo gerar como resultado ao usuário as informações: 'Token incorreto' ou 'Token expirado'
- (Technical debt) Implementar os testes unitários das classes contidas em libs/Shared; especificamente, das classes ApplicationResponse, PaginationOptions, Inconsistency e CnpjValidator
- (Technical debt) Implementar os testes unitários dos service client de integração com o serviço ViaCep
- (Technical debt) Implementar os testes de integração dos endpoints:<br/>
  GetLeadById<br/>
  RegisterLead<br/>
  RemoveLead<br/>
  UpdateLead<br/>
  SearchLead<br/>
  BulkInsertLead<br/>
- (Technical debt) Utilizar StronglyTypedIds na camada de Entidades
- (Technical debt) Adicionar HealthChecks, incluindo endpoint na API
- (Technical debt) Adicionar um endpoint de métricas, pronto para o Prometheus realizar 'scrapings'
- (Technical debt) Integrar a aplicação com alguma ferramenta de telemetria; preferência pelo uso do Data Dog e/ou Jaeger

Em termos de implementação, o que tem de reaproveitável no código-fonte deste projeto e/ou que de repente pode servir como ponto de partida para outros projetos?
- Estruturação de pastas focado em funcionalidades (casos de uso da aplicação) de maneira que inclusive seja muito fácil encontrar classes de Handlers, Validação, Requests, Testes unitários e de Integração correspondentes
- Classe base Controller com abstrações para retornos dos endpoints
- Classe base de Rota para centralizar prefixos de rota
- Classe ActionFilter assíncrona de validação de Api Key, permitindo ou negando acesso aos endpoints da API
- Classe Middleware de manipulação da solicitações http, contendo lógica de manipulação de exceção e preparo de respostas http de maneira padronizada
- Estrutura de dados ApiResponse/ApiResponse<T> para padronização das respostas http
- Classe de extensão de validação de Cep integrada ao FluentValidations
- Classe de extensão de validação de Cnpj integrada ao FluentValidations
- Classe de com lógica de validação de Cnpj utilizando o algoritmo Módulo 1
- Classes, estruturas e métodos de extensão LINQ para realização de paginação de dados - inclusive com ordenação (PaginationOptions, PagedList, EnumerableExtensions - ToPagedList, ToSortedList, ToSortedPagedList)
  - Com o método ToSortedList é possível passar o nome da coluna (passada como string) com a qual se deseja realizar a ordenação. Internamente, o método Lambda.Expression(...).Compile() é utilizado para a realização da 'mágica'.
- Implementação de um service client de integração com o serviço de localização de endereços ViaCep
- Abstrações para manipulação de eventos:
  - IEvent
    - IDomainEvent
    - IIntegrationEvent
- Classes que fazem forte uso da biblioteca MediatR:
  - Classes base para Command and Query Handlers e Notification Handlers
  - Command and Query handlers
  - Pipeline Behaviors
    - ValidationBehavior
  - Post Processors
    - EventHandlerDispatchingProcessor
  - Notification handlers
    - Domain event handlers
    - Integration event handlers
- Classe emissora de eventos EventDispatcher
- Classes de extensão de injeção de dependência das camadas Api, Application e Infrastructure para ótima manutenibilidade da lógica de configuração da aplicação web (AddApiServices, AddApplicationServices, AddInfrastructureServices)
- Implementação de classes ObjectMother de construção de Requests e Entidades para ótima manutenibilidade das suítes de testes
- Implementação de classe Factory de DbContext em memória para execução dos testes de integração da Api
- Lógica de auditoria durante o processo de persistência dos dados em LeadsDbContext, gravando o usuário e data/hora da operação (em formato UTC e baseado no GMT inclusive)
- Métodos de testes unitários - inclusive com uso de asserções fluentes - de:<br/>
  Classes Handlers<br/>
  Classes Validators<br/>
  Classes EventHandler<br/>
  Entidades<br/>
  Classes de estruturas de dados comuns, como ApplicationResponse<T><br/>
- Métodos de testes de integração - inclusive com uso de asserções fluentes - de endpoints de API
- Classes WebApplicationFactory e ClassFixtures
- Classe InMemoryLeadManagerDbContextFactory que ajuda na execução de testes com banco de dados em memória<br/>
- Integração com o serviço de armazenamento do Azure (Blob Storage) através da classe BlobStorageProvider para upload de arquivos de lotes de Leads
  - Contém lógica de política de retentativas com 'back-off' exponencial através do uso da biblioteca Polly
(Continuar a listagem. Afinal, tem muita coisa que vale anotar aqui como índice/referência!)
- Lógica de configuração do tamanho máximo de upload de arquivos
- Abstrações para utilização de cache de dados com Redis para ganho de performance
  - ICacheProvider
    - Classe que implementa integração com StackExchange Redis, serializando e deserializando inclusive utilizando Protobuf para velocidade e economia de espaço em memória com estes processos
  - ICachingManagement (que no final das contas é um repositório de dados preenchido com dados da fonte de dados)
- Abstrações para integração com serviço de mensageria RabbitMQ

Lista com os principais pacotes Nuget que foram utilizados neste projeto:<br/>
- LanguageExt.Core
- MediatR
- FluentValidaton
- Microsoft.EntityFrameworkCore
- Microsoft.EntityFrameworkCore.Design
- Microsoft.EntityFrameworkCore.SqlServer
- Microsoft.EntityFrameworkCore.Tools
- Microsoft.Extensions.Http.Polly
- Polly
- Microsoft.Extensions.Caching.StackExchangeRedis
- protobuf-net
- RabbitMQ.Client
- RichardSzalay.MockHttp
- Microsoft.AspNetCore.Mvc.Testing
- Microsoft.Data.Sqlite.Core
- Microsoft.EntityFrameworkCore.InMemory
- Serilog
- NSubstitute
- coverlet.collector
- xunit
- FluentAssertions

Lista com os principais utilitários de linha de comando que foram utilizados neste projeto:<br/>
- dotnet
    dotnet restore<br/>
    dotnet build<br/>
    dotnet ef<br/>
    dotnet test<br/>
    reportgenerator (dotnet-reportgenerator-globaltool)<br/>
- docker
- git
 
O projeto está em constante evolução e sempre pode ser melhorado, tanto em termos de organização (estrutura de pastas, separações de responsabilidades) quanto de algoritmos dentre outras coisas! Portanto, opiniões são sempre muito bem-vindas! :)
