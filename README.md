 # Radical Snake Game

## Resumo do Jogo
"Radical Snake Game" é um jogo de sobrevivência onde o jogador controla uma cobra azul e enfrenta ondas de inimigos cada vez mais desafiadoras. O objetivo é sobreviver até a última onda, derrotando inimigos e evitando que eles colidam com a cobra. Ao longo das ondas, inimigos se tornam mais rápidos e surgem em maior quantidade, tornando a experiência cada vez mais intensa e estratégica. Para ajudar na sobrevivência, o jogador pode coletar frutas que restauram a vida da cobra.

## Como o Tema Foi Abordado
O tema "Built to Scale" foi implementado através de um sistema de ondas (waves) progressivas. A cada nova onda:
- A quantidade de inimigos aumenta.
- A velocidade dos inimigos também cresce, tornando mais difícil desviar ou derrotá-los.
- O jogador é desafiado a utilizar suas habilidades de movimentação e estratégia para sobreviver.

Além disso, um sistema de vitória foi incluído para recompensar o jogador ao final da última onda (wave 10), quando uma tela de vitória é exibida.

## Como Executar o Projeto

1. **Clone o repositório**:
   Abra o terminal e execute o seguinte comando para clonar o repositório:
   ```bash
   git clone <URL do repositório>
   ```

2. **Abra o projeto na Unity**:
   - Certifique-se de ter a Unity instalada (versão recomendada: 2022.3.17f1 ou superior).
   - Abra a Unity Hub, clique em "Open" e selecione a pasta do projeto clonada.

3. **Execute o jogo**:
   - No editor da Unity, abra a cena principal do jogo (geralmente chamada de "MainScene" ou similar).
   - Pressione o botão "Play" no editor para iniciar o jogo.

4. **Exportação para jogar fora do editor** (opcional):
   - Para exportar o jogo como um executável, acesse o menu "File > Build Settings...".
   - Selecione a plataforma desejada (Windows, macOS, etc.), configure as opções e clique em "Build".

# Controles do Jogo

- **Rotação do Player:** Use as setas horizontais ou as teclas **A/D** para rotacionar o player.
- **Ocultar/Tornar Visível o Cursor do Mouse:** Pressione a tecla **Space**.
- **Mira e Tiro:** Use o cursor do mouse para mirar e clique com o botão esquerdo do mouse para atirar.

Se encontrar problemas ou tiver dúvidas, verifique a documentação do código e os comentários implementados.


