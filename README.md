# Projeto: Lead Manager (Back-end)

O que é o Lead Manager?<br/>
É um projeto que tem como objetivo permitir gerenciar de maneira simples e intuitiva - através de operações de listagem, adiçāo, atualizaçāo e remoção - dados de leads.
A parte de front-end do projeto consiste atualmente em duas telas.
Uma para listagem de leads que, a partir dela, os usuários são capazes de:
- Visualizar uma lista contendo os dados principais de leads existentes
- Acionar o botão para adicionar novos leads
- Selecionar um lead a fim de removê-lo ou
- Selecionar um lead e acionar o botão para atualizar os respectivos dados<br/>
E outra para as operações de adicionar um novo lead ou atualizar um lead previamente selecionado

O Lead Manager é um projeto que utiliza as seguintes linguagens, tecnologias, funcionalidades e ferramentas:
- .Net Core 7
- Linguagem C#
- Orientação a objetos
- Práticas de Código limpo / Clean code
- Arquitetura Limpa / Clean architecture, constituída pelas camadas: Core, Application, Infrastructure e Presentation
- Programação assíncrona baseada em Tasks
- Asp.Net Core Web Api
- Swagger
- Entity Framework Core
- Sql Server
- Sqlite
- Testes unitários / Unit tests / TDD com xUnit e Fluent Assertions
- Testes de integração / Integration tests com WebApplicationFactory e MockHttp
- Integração com o serviço de localização de endereços ViaCep
- Aplicação de princípios SOLID
- Aplicação de padrões de projeto / design patterns como: Object mother, Singleton, Mediator, Factory, Factory method
- Aplicação do padrão arquitetural CQRS de maneira lógica com MediatR
- Uso de Pipeline Behaviors para validação de dados de entrada em combinação com FluentValidations
- Uso de Design dirigido a domínio / domain-driven design / DDD para modalagem das entidades e dos comportamentos da classes e objetos de valor

Pré-requisitos para execução do Front-End da aplicação<br/>
É necessário possuir os seguintes componentes instalados na máquina:
- SDK do .Net Core 7 (que pode ser obtido através da url: https://nodejs.org/en](https://dotnet.microsoft.com/en-us/download/dotnet/7.0)
- Docker<br/>
  Se sua máquina for Mac, você pode seguir os passos conforme a url: https://docs.docker.com/desktop/install/mac-install/<br/>
  Se sua máquina for Linux, você pode seguir os passos conforme a url: https://docs.docker.com/desktop/install/linux-install/#generic-installation-steps<br/>
  Se sua máquina for Windows, você pode seguir os passos conforme a url: https://docs.docker.com/desktop/install/windows-install/<br/>

Como executar o projeto localmente?
- Garanta que a máquina esteja devidamente configurada, conforme a seção "Pré-requisitos para execução do Front-End da aplicação"
- Execute o Docker
- Acesse o Terminal, Command Prompt ou Powershell
- Execute o seguinte comando para subir um servidor de banco de dados Sql Server
  docker run -e "MSSQL_PID=Express" -e "ACCEPT_EULA=Y" -e "SA_PASSWORD=Y0urStr0nGP@sswoRD_2023" -p 1433:1433 --name leadmanager-db -d mcr.microsoft.com/mssql/server
- Navegue até a pasta raíz do projeto (mesma pasta que contém o arquivo LeadManager.sln, por exemplo)
- Execute o seguinte comando:
  dotnet build
  dotnet run
  (O comando irá compilar a solução, subirá um servidor Kestrel e automaticamente abrirá o navegador web padrão apontado para o Swagger da API)
NOTA: Em breve, essas etapas para execução da aplicação na máquina local serão simplificadas a uma única linha de comando através do uso de Docker-Compose.

Novas demandas no radar:
- (User Story) Adicionar endpoint para receber arquivo de leads em formato CSV para fins de adição em lote
- (User Story) Proteger a API contra acesso indevido, de maneira que somente usuário autenticados possam invocar os endpoints
  - Possibilidade 1: a aplicação deverá ser capaz de encaminhar a solicitação de autenticação para um servidor de identidade a fim de obter o Token de autenticação
  - Possibilidade 2: a aplicação deverá ser capaz de validar tokens de autenticação/autorização - incluindo Claims - que possibilitem ou recusem executar os endpoints da API
- (Technical debt) Criar Dockerfile do projeto
- (Technical debt) Adicionar a aplicação ao Docker-Compose para simplificar a configuração da máquina e permitir automatizar a execução da mesma em uma única linha de comando

Em termos de implementação, o que tem de reaproveitável no código-fonte deste projeto e/ou que de repente pode servir como ponto de partida ou para outros projetos?
- Estruturação de pastas focado em funcionalidades (casos de uso da aplicação) de maneira que inclusive seja muito fácil encontrar classes de Handlers, Validação, Requests, Testes unitários e de Integração correspondentes<br/>
- Classe base Controller com abstrações para retornos dos endpoints
- Classe base de Rota para centralizar prefixos de rota
- Classe ActionFilter assíncrona de validação de Api Key, permitindo ou negando acesso aos endpoints da API
- Classe Middleware de manipulação da solicitações http, contendo lógica de manipulação de exceção e preparo de respostas http de maneira padronizada
- Estrutura de dados ApiResponse/ApiResponse<T> para padronização das respostas http
- Classe de extensão de validação de Cep integrada ao FluentValidations
- Classe de extensão de validação de Cnpj integrada ao FluentValidations
- Classe de com lógica de validação de Cnpj utilizando o algoritmo Módulo 1
- Implementação de um service client de integração com o serviço de localização de endereços ViaCep
- Classes de exensão de injeção de dependência das camadas Api, Application e Infrastructure para ótima manutenibilidade da lógica de configuração da aplicação web (AddApiServices, AddApplicationServices, AddInfrastructureServices)
- Implementação de classes ObjectMother de construção de Requests e Entidades para ótima manutenibilidade das suítes de testes
- Implementação de classe Factory de DContext em memória para execução dos testes de integração da Api
- Lógica de auditoria durante o processo de persistência dos dados em LeadsDbContext, gravando o usuário e data/hora da operação
  
  (Continuar a listagem. Afinal, tem muita coisa que vale anotar aqui como índice/referência!)
  
O projeto está em processo de evolução e sempre pode ser melhorado, tanto em termos de organização (estrutura de pastas, separações de responsabilidades) quanto de algoritmos, usos de elementos Angular mais adequados para situações específicas dentre outras coisas! Portanto, opiniões sempre são muito bem-vindas! :)
