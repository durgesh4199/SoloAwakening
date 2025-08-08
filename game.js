const canvas = document.getElementById('gameCanvas');
const ctx = canvas.getContext('2d');

const player = { x: 300, y: 200, size: 20, speed: 3 };

const tokens = [
  { x: 80, y: 80, radius: 10, message: 'Collect data: gather text!' },
  { x: 520, y: 90, radius: 10, message: 'Tokenize: split text into pieces!' },
  { x: 120, y: 320, radius: 10, message: 'Train: learn from tokens!' },
  { x: 470, y: 270, radius: 10, message: 'Generate: create new text!' }
];

let keys = {};
let collectedCount = 0;

function drawPlayer() {
  ctx.fillStyle = 'blue';
  ctx.fillRect(player.x - player.size / 2, player.y - player.size / 2, player.size, player.size);
}

function drawTokens() {
  ctx.fillStyle = 'green';
  tokens.forEach(t => {
    if (!t.collected) {
      ctx.beginPath();
      ctx.arc(t.x, t.y, t.radius, 0, Math.PI * 2);
      ctx.fill();
    }
  });
}

function movePlayer() {
  if (keys['ArrowUp']) player.y -= player.speed;
  if (keys['ArrowDown']) player.y += player.speed;
  if (keys['ArrowLeft']) player.x -= player.speed;
  if (keys['ArrowRight']) player.x += player.speed;

  player.x = Math.max(player.size / 2, Math.min(canvas.width - player.size / 2, player.x));
  player.y = Math.max(player.size / 2, Math.min(canvas.height - player.size / 2, player.y));
}

function checkCollisions() {
  tokens.forEach(t => {
    if (!t.collected) {
      const dx = player.x - t.x;
      const dy = player.y - t.y;
      const distance = Math.sqrt(dx * dx + dy * dy);
      if (distance < player.size / 2 + t.radius) {
        t.collected = true;
        collectedCount++;
        alert(t.message);
        if (collectedCount === tokens.length) {
          trainAndGenerate();
        }
      }
    }
  });
}

function loop() {
  ctx.clearRect(0, 0, canvas.width, canvas.height);
  movePlayer();
  drawPlayer();
  drawTokens();
  checkCollisions();
  requestAnimationFrame(loop);
}

function trainBigramModel(text) {
  const model = {};
  for (let i = 0; i < text.length - 1; i++) {
    const ch1 = text[i];
    const ch2 = text[i + 1];
    if (!model[ch1]) model[ch1] = {};
    model[ch1][ch2] = (model[ch1][ch2] || 0) + 1;
  }
  return model;
}

function generateText(model, start, length) {
  let result = start;
  let current = start;
  for (let i = 0; i < length; i++) {
    const nextOptions = model[current];
    if (!nextOptions) break;
    const total = Object.values(nextOptions).reduce((a, b) => a + b, 0);
    let r = Math.random() * total;
    for (const [char, count] of Object.entries(nextOptions)) {
      r -= count;
      if (r <= 0) {
        result += char;
        current = char;
        break;
      }
    }
  }
  return result;
}

function trainAndGenerate() {
  const sample = 'hello world. the world is full of words. learn and build models.';
  const model = trainBigramModel(sample);
  const text = generateText(model, 'h', 80);
  alert('Tiny model says:\n' + text);
}

window.addEventListener('keydown', e => keys[e.key] = true);
window.addEventListener('keyup', e => keys[e.key] = false);

loop();
