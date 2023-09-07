# Funcionamento

Foi pedido que fizesse um sistema no âmbito de locação de imóveis. A partir disso, a forma como o sistema foi projetado para funcionar é a seguinte:
1. Usuário se registra ou faz login na plataforma;
2. Usuário cadastra um imóvel no qual pretende locar;
3. Usuário cadastra uma locação, especificando qual o imóvel, o locatário, a data de vencimento e o valor mensal da locação;
4. Usuário que cadastrou a locação a assina (o sistema trata locações como um contrato);
5. Usuário locatário que foi especificado na locação cadastrada anteriormente assina a locação;
6. Com isso, a locação foi assinada por ambas as partes e é preenchido o valor na data de fechamento como a data atual, indicando quando a locação foi assinada por ambas as partes.

# Arquitetura

O sistema foi feito baseado na Clean Architecture e foi dividido em 4 camadas, sendo elas: Aplicação, Infraestrutura, Domínio e Api.

### Aplicação

É responsável pela lógica de negócios da aplicação. Ela realiza validações, mapeia dados entre entidades de domínio e DTOs ou vice-versa, controla o fluxo de execução e faz chamadas à camada de Infraestrutura para obter dados de serviços externos como APIs de terceiros ou realizar acesso ao banco de dados. Além disso, é onde as interfaces são definidas.

### Domínio

É o core da aplicação, onde todas as regras de negócio são definidas. Ela contém as entidades de domínio e a lógica de negócios que operam sobre essas entidades.

### Infraestrutura

Fornece as implementações de acesso a serviços externos, como acesso a bancos de dados, chamadas a APIs externas, etc.

### Api

Essa camada é responsável por expor as funcionalidades da aplicação para os usuários. Ela trata de roteamento de solicitações, serialização/desserialização de dados e interação com a camada de Aplicação, chamando seus serviços.

# Testes unitários

Os testes unitários foram feitos utilizando XUnit e NSubstitute para o mock de dependências. Foram utilizadas classes geradoras de dados, como por exemplo a classe GeradorImovel e classes de constantes para cada entidade, mantendo os valores padronizados.

#  Rodar o projeto

Para rodar o projeto, primeiramente é necessário executar as migrations. Para isso, entre na pasta de Infraestrutura e execute o seguinte comando:
```bash
dotnet ef --startup-project ../Api/Api.csproj database update
```
Com isso, basta rodar o projeto com o seguinte comando:
```bash
dotnet run
```
Ao visitar a página no swagger, é possível interagir com todos os endpoints e testar a aplicação.

# Observações
1. Para inserir o token JWT no swagger, não é necessário adicionar o prefixo de Bearer, basta adicionar o token retornado pelo sistema;
2. Os arquivos de configurações que possuem dados sensíveis foram adicionados ao repositório propositalmente com o intuito de facilitar a criação do banco e dos testes da aplicação.
