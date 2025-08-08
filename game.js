const canvas = document.getElementById('gameCanvas');
const ctx = canvas.getContext('2d');
ctx.textAlign = 'center';
ctx.textBaseline = 'middle';

let currentGame = null;

const angleSlider = document.getElementById('angleSlider');
const tokenTownBtn = document.getElementById('tokenTownBtn');
const attentionArenaBtn = document.getElementById('attentionArenaBtn');
const backBtn = document.getElementById('backBtn');

tokenTownBtn.addEventListener('click', startTokenTown);
attentionArenaBtn.addEventListener('click', startAttentionArena);
backBtn.addEventListener('click', showMenu);
angleSlider.addEventListener('input', () => {
  if (currentGame === 'attentionArena') {
    attentionArena.queryAngle = parseInt(angleSlider.value, 10);
  }
});

function showMenu() {
  currentGame = null;
  angleSlider.style.display = 'none';
  canvas.onclick = null;
  tokenTownBtn.style.display = 'inline';
  attentionArenaBtn.style.display = 'inline';
  backBtn.style.display = 'none';
}

// ---------------- Token Town ----------------
const tokenTown = {
  word: 'TRANSFORMER',
  cuts: Array('TRANSFORMER'.length - 1).fill(false),
  budget: 5
};

let tokenTownSuccess = false;

function startTokenTown() {
  currentGame = 'tokenTown';
  angleSlider.style.display = 'none';
  tokenTownBtn.style.display = 'none';
  attentionArenaBtn.style.display = 'none';
  backBtn.style.display = 'inline';
  canvas.onclick = tokenTownClick;
  tokenTown.cuts.fill(false);
  tokenTownSuccess = false;
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

function getTokens(word, cuts) {
  const tokens = [];
  let current = '';
  for (let i = 0; i < word.length; i++) {
    current += word[i];
    if (cuts[i]) {
      tokens.push(current);
      current = '';
    }
  }
  tokens.push(current);
  return tokens;
}

function drawTokenTown() {
  const startX = 60;
  const spacing = (canvas.width - 120) / (tokenTown.word.length - 1);
  const y = canvas.height / 2;
  const tokensArr = getTokens(tokenTown.word, tokenTown.cuts);
  const colors = ['#7b3fb3', '#985eff', '#bb86fc', '#ff79c6'];
  let index = 0;
  ctx.font = '32px Arial';
  for (let t = 0; t < tokensArr.length; t++) {
    const tok = tokensArr[t];
    const rectX = startX + index * spacing - spacing / 2;
    const rectW = spacing * tok.length;
    ctx.fillStyle = colors[t % colors.length];
    ctx.fillRect(rectX, y - 40, rectW, 80);
    for (let j = 0; j < tok.length; j++) {
      const x = startX + (index + j) * spacing;
      ctx.fillStyle = '#fff';
      ctx.fillText(tok[j], x, y);
    }
    index += tok.length;
  }
  for (let i = 0; i < tokenTown.cuts.length; i++) {
    const cutX = startX + (i + 0.5) * spacing;
    ctx.strokeStyle = tokenTown.cuts[i] ? '#ff79c6' : '#444';
    ctx.beginPath();
    ctx.moveTo(cutX, y - 40);
    ctx.lineTo(cutX, y + 40);
    ctx.stroke();
  }
  const tokenCount = tokensArr.length;
  ctx.fillStyle = '#fff';
  ctx.fillText(`Tokens: ${tokenCount} / ${tokenTown.budget}`, canvas.width / 2, 40);
  if (tokenCount > tokenTown.budget) {
    ctx.fillStyle = '#ff5555';
    ctx.fillText('Over budget!', canvas.width / 2, 80);
    tokenTownSuccess = false;
  } else {
    ctx.fillStyle = '#50fa7b';
    ctx.fillText('Within budget.', canvas.width / 2, 80);
    if (!tokenTownSuccess) {
      spawnConfetti();
      tokenTownSuccess = true;
    }
  }
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

let attentionSuccess = false;

function startAttentionArena() {
  currentGame = 'attentionArena';
  angleSlider.style.display = 'inline';
  tokenTownBtn.style.display = 'none';
  attentionArenaBtn.style.display = 'none';
  backBtn.style.display = 'inline';
  canvas.onclick = null;
  attentionArena.queryAngle = parseInt(angleSlider.value, 10);
  attentionSuccess = false;
}

function drawAttentionArena() {
  ctx.font = '24px Arial';
  const spacing = canvas.width / (attentionArena.words.length + 1);
  const y = canvas.height / 2;

  const dots = attentionArena.words.map(w => Math.cos((attentionArena.queryAngle - w.angle) * Math.PI / 180));
  const exps = dots.map(Math.exp);
  const sum = exps.reduce((a, b) => a + b, 0);
  const weights = exps.map(e => e / sum);

  const centerX = canvas.width / 2;
  const centerY = canvas.height - 60;
  for (let i = 0; i < attentionArena.words.length; i++) {
    const x = spacing * (i + 1);
    const w = weights[i];
    const lineColor = `rgba(138,43,226,${w})`;
    ctx.strokeStyle = lineColor;
    ctx.lineWidth = w * 10;
    ctx.beginPath();
    ctx.moveTo(centerX, centerY);
    ctx.lineTo(x, y);
    ctx.stroke();

    ctx.fillStyle = '#fff';
    ctx.fillText(attentionArena.words[i].text, x, y);
    ctx.fillStyle = '#bbb';
    ctx.fillText(w.toFixed(2), x, y - 40);
  }

  // draw query vector arrow
  const length = 50;
  const rad = attentionArena.queryAngle * Math.PI / 180;
  ctx.strokeStyle = '#8a2be2';
  ctx.beginPath();
  ctx.moveTo(centerX, centerY);
  ctx.lineTo(centerX + length * Math.cos(rad), centerY + length * Math.sin(rad));
  ctx.stroke();

  if (weights[3] >= 0.6) {
    ctx.fillStyle = '#50fa7b';
    ctx.fillText('Success! FOX is focused.', canvas.width / 2, 40);
    if (!attentionSuccess) {
      spawnConfetti();
      attentionSuccess = true;
    }
  } else {
    ctx.fillStyle = '#ff5555';
    ctx.fillText('Adjust query to focus on FOX (≥ 0.60)', canvas.width / 2, 40);
    attentionSuccess = false;
  }
}

let confetti = [];
let score = 0;

function spawnConfetti() {
  for (let i = 0; i < 20; i++) {
    confetti.push({
      x: canvas.width / 2,
      y: canvas.height / 2,
      dx: Math.random() * 4 - 2,
      dy: Math.random() * -4,
      size: 4,
      color: `hsl(${Math.random() * 360},100%,70%)`,
      life: 60
    });
  }
  score++;
}

function updateConfetti() {
  for (let i = confetti.length - 1; i >= 0; i--) {
    const p = confetti[i];
    p.x += p.dx;
    p.y += p.dy;
    p.dy += 0.1;
    p.life--;
    if (p.life <= 0) {
      confetti.splice(i, 1);
    } else {
      ctx.fillStyle = p.color;
      ctx.fillRect(p.x, p.y, p.size, p.size);
    }
  }
}

function loop() {
  requestAnimationFrame(loop);
  ctx.fillStyle = '#2e004f';
  ctx.fillRect(0, 0, canvas.width, canvas.height);
  if (currentGame === 'tokenTown') {
    drawTokenTown();
  } else if (currentGame === 'attentionArena') {
    drawAttentionArena();
  } else {
    ctx.fillStyle = '#fff';
    ctx.font = '20px Arial';
    ctx.fillText('Choose a mini-game to begin.', canvas.width / 2, canvas.height / 2);
  }
  updateConfetti();
  ctx.fillStyle = '#fff';
  ctx.font = '16px Arial';
  ctx.fillText(`Score: ${score}`, 60, 20);
}

loop();
