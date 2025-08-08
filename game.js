const canvas = document.getElementById('gameCanvas');
const ctx = canvas.getContext('2d');
ctx.textAlign = 'center';
ctx.textBaseline = 'middle';

let currentGame = null;

const angleSlider = document.getElementById('angleSlider');
const tokenTownBtn = document.getElementById('tokenTownBtn');
const attentionArenaBtn = document.getElementById('attentionArenaBtn');

tokenTownBtn.addEventListener('click', startTokenTown);
attentionArenaBtn.addEventListener('click', startAttentionArena);
angleSlider.addEventListener('input', () => {
  if (currentGame === 'attentionArena') {
    attentionArena.queryAngle = parseInt(angleSlider.value, 10);
  }
});

// ---------------- Token Town ----------------
const tokenTown = {
  word: 'TRANSFORMER',
  cuts: Array('TRANSFORMER'.length - 1).fill(false),
  budget: 5
};

function startTokenTown() {
  currentGame = 'tokenTown';
  angleSlider.style.display = 'none';
  canvas.onclick = tokenTownClick;
  tokenTown.cuts.fill(false);
}

function tokenTownClick(evt) {
  const rect = canvas.getBoundingClientRect();
  const x = evt.clientX - rect.left;
  const startX = 60;
  const spacing = (canvas.width - 120) / (tokenTown.word.length - 1);
  for (let i = 0; i < tokenTown.cuts.length; i++) {
    const cutX = startX + (i + 0.5) * spacing;
    if (Math.abs(x - cutX) < 10) {
      tokenTown.cuts[i] = !tokenTown.cuts[i];
      break;
    }
  }
}

function drawTokenTown() {
  ctx.clearRect(0, 0, canvas.width, canvas.height);
  const startX = 60;
  const spacing = (canvas.width - 120) / (tokenTown.word.length - 1);
  const y = canvas.height / 2;
  ctx.font = '32px Arial';
  for (let i = 0; i < tokenTown.word.length; i++) {
    const x = startX + i * spacing;
    ctx.fillStyle = 'black';
    ctx.fillText(tokenTown.word[i], x, y);
  }
  for (let i = 0; i < tokenTown.cuts.length; i++) {
    const cutX = startX + (i + 0.5) * spacing;
    ctx.strokeStyle = tokenTown.cuts[i] ? 'red' : '#ccc';
    ctx.beginPath();
    ctx.moveTo(cutX, y - 40);
    ctx.lineTo(cutX, y + 40);
    ctx.stroke();
  }
  const tokens = 1 + tokenTown.cuts.filter(Boolean).length;
  ctx.fillStyle = 'black';
  ctx.fillText(`Tokens: ${tokens} / ${tokenTown.budget}`, canvas.width / 2, 40);
  ctx.fillStyle = tokens > tokenTown.budget ? 'red' : 'green';
  ctx.fillText(tokens > tokenTown.budget ? 'Over budget!' : 'Within budget.', canvas.width / 2, 80);
}

// ---------------- Attention Arena ----------------
const attentionArena = {
  words: [
    { text: 'THE', angle: 20 },
    { text: 'QUICK', angle: 110 },
    { text: 'BROWN', angle: 200 },
    { text: 'FOX', angle: 290 }
  ],
  queryAngle: 0
};

function startAttentionArena() {
  currentGame = 'attentionArena';
  angleSlider.style.display = 'inline';
  canvas.onclick = null;
  attentionArena.queryAngle = parseInt(angleSlider.value, 10);
}

function drawAttentionArena() {
  ctx.clearRect(0, 0, canvas.width, canvas.height);
  ctx.font = '24px Arial';
  const spacing = canvas.width / (attentionArena.words.length + 1);
  const y = canvas.height / 2;

  const dots = attentionArena.words.map(w => Math.cos((attentionArena.queryAngle - w.angle) * Math.PI / 180));
  const exps = dots.map(Math.exp);
  const sum = exps.reduce((a, b) => a + b, 0);
  const weights = exps.map(e => e / sum);

  for (let i = 0; i < attentionArena.words.length; i++) {
    const x = spacing * (i + 1);
    const w = weights[i];
    ctx.fillStyle = (attentionArena.words[i].text === 'FOX' && w >= 0.6) ? 'green' : 'black';
    ctx.fillText(attentionArena.words[i].text, x, y);
    ctx.fillStyle = 'blue';
    ctx.fillText(w.toFixed(2), x, y - 40);
  }

  // draw query vector arrow
  const centerX = canvas.width / 2;
  const centerY = canvas.height - 60;
  const length = 50;
  const rad = attentionArena.queryAngle * Math.PI / 180;
  ctx.strokeStyle = 'black';
  ctx.beginPath();
  ctx.moveTo(centerX, centerY);
  ctx.lineTo(centerX + length * Math.cos(rad), centerY + length * Math.sin(rad));
  ctx.stroke();

  if (weights[3] >= 0.6) {
    ctx.fillStyle = 'green';
    ctx.fillText('Success! FOX is focused.', canvas.width / 2, 40);
  } else {
    ctx.fillStyle = 'red';
    ctx.fillText('Adjust query to focus on FOX (≥ 0.60)', canvas.width / 2, 40);
  }
}

function loop() {
  requestAnimationFrame(loop);
  if (currentGame === 'tokenTown') {
    drawTokenTown();
  } else if (currentGame === 'attentionArena') {
    drawAttentionArena();
  } else {
    ctx.clearRect(0, 0, canvas.width, canvas.height);
    ctx.fillStyle = 'black';
    ctx.font = '20px Arial';
    ctx.fillText('Choose a mini-game to begin.', canvas.width / 2, canvas.height / 2);
  }
}

loop();
