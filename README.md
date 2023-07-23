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
- NodeJS 18 (que pode ser obtido através da url: https://nodejs.org/en)
- Angular CLI
  Após instalado o NodeJs, digitar a seguinte linha de comando no Terminal, Command Prompt ou Powershell:<br/>
    npm install -g @angular/cli [ENTER]<br/>
    NOTA: Caso o sistema operacional seja o Linux ou MacOs, poderá ser necessário prefixar a linha de comando com sudo e, em seguida, ter que informar a senha de administrador para prosseguir com a instalação.

Como executar o projeto localmente?
- Garanta que a máquina esteja devidamente configurada, conforme a seção "Pré-requisitos para execução do Front-End da aplicação"
- Acesse o Terminal, Command Prompt ou Powershell
- Navegue até a pasta raíz do projeto (mesma pasta que contém o arquivo package.json, por exemplo)
- Execute o seguinte comando:
  ng serve -o
  (O comando irá gerar os arquivos necessários para execução e automaticamente abrirá o navegador web padrão com a Url da aplicação).

Novas demandas no radar:
- (User Story) Adicionar endpoint para receber arquivo CSV de leads para fins de adição em lote
- (User Story) Proteger a API contra acesso indevido, de maneira que somente usuário autenticados possam invocar os endpoints
  - Possibilidade 1: a aplicação deverá ser capaz de encaminhar a solicitação de autenticação para um servidor de identidade a fim de obter o Token de autenticação
  - Possibilidade 2: a aplicação deverá ser capaz de validar tokens de autenticação/autorização - incluindo Claims - que possibilitem ou recusem executar os endpoints da API
- (Technical debt) Criar Dockerfile do projeto
- (Technical debt) Adicionar a aplicação ao Docker-Compose para simplificar a configuração da máquina e permitir automatizar a execução da mesma em uma única linha de comando

Em termos de implementação, o que tem de reaproveitável no código-fonte deste projeto e/ou que de repente pode servir como ponto de partida ou para outros projetos?
- Práticas de Clean Code<br/>
- Estruturação de pastas de maneira um pouco similar a projetos de back-end implementados com Clean Architecture, agrupando implementações por features (List leads, Maintain Lead)
- No formulário de gerenciamento de Leads (app/leads/maintain/views/maintain-lead.*)
  Formulário digirido a modelo
  Lógica de construção do formulário com FormBuilder<br/>
  Uso de validadores síncronos (app/common/validation/custom-validators.ts)<br/>
    Validação de Cnpj através de Regex e através do algoritmo baseado no Módulo 1<br/>
    Validação de Cep baseado em Regex<br/>
  Uso de validadores assíncronos (app/common/validation/custom-validators.ts)<br/>
    Disparo de solicitação ao servidor para validação do Cnpj<br/>
    Disparo de solicitação ao servidor para validação da Razão social<br/>
  Uso de diretivas de elementos html (app/common/ui/input-masks/cep-mask.directive.ts + cnpj-mask.directive.ts)<br/>
    Formatação automática/dinâmica dos campos Cnpj e Cep<br/>
    Lógica de configuração no campo Cep para buscar o endereço relacionado<br/>
  Lógica que exibe Dialog de confirmação de abandono de página caso o formulário de lead possua informações não confirmada com Guards (app/common/ui/navigation/on-leave.ts + leave-confirmation.guard.ts | app/leads/maintain/views/maintain-lead.ts)<br/>
- Interceptador Http para manipular solicitações Http e tratar situações de erro específicas (app/common/infrastructure/error-handling.interceptor.ts)<br/>
  Uso da Api HttpClient<br/>
  Forte uso de operadores RxJs<br/>
  Uso do serviço de indicação de progresso da solicitação<br/>
  Tratamento de exceções específicas (erros técnicos e erros de domínio)<br/>
  
  (Continuar a listagem. Afinal, tem muita coisa que vale anotar aqui como índice/referência!)
  
O projeto está em processo de evolução e sempre pode ser melhorado, tanto em termos de organização (estrutura de pastas, separações de responsabilidades) quanto de algoritmos, usos de elementos Angular mais adequados para situações específicas dentre outras coisas! Portanto, opiniões sempre são muito bem-vindas! :)
