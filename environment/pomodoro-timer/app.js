// Configurazione del timer Pomodoro
const WORK_TIME = 25 * 60; // 25 minuti in secondi
const SHORT_BREAK = 5 * 60; // 5 minuti in secondi
const LONG_BREAK = 15 * 60; // 15 minuti in secondi
const LONG_BREAK_INTERVAL = 4; // Dopo 4 work sessions

let timer;
let isRunning = false;
let timeLeft = WORK_TIME;
let currentMode = 'work'; // work, shortBreak, longBreak
let sessionsCompleted = 0;

// Elementi DOM
const minutesDisplay = document.getElementById('minutes');
const secondsDisplay = document.getElementById('seconds');
const modeDisplay = document.getElementById('mode');
const startBtn = document.getElementById('startBtn');
const pauseBtn = document.getElementById('pauseBtn');
const resetBtn = document.getElementById('resetBtn');
const sessionCount = document.getElementById('sessionCount');

// Funzione per aggiornare il display del timer
function updateDisplay() {
    const minutes = Math.floor(timeLeft / 60);
    const seconds = timeLeft % 60;
    minutesDisplay.textContent = minutes.toString().padStart(2, '0');
    secondsDisplay.textContent = seconds.toString().padStart(2, '0');
}

// Funzione per aggiornare l'etichetta della modalità
function updateModeLabel() {
    switch(currentMode) {
        case 'work':
            modeDisplay.textContent = 'Lavoro';
            break;
        case 'shortBreak':
            modeDisplay.textContent = 'Pausa breve';
            break;
        case 'longBreak':
            modeDisplay.textContent = 'Pausa lunga';
            break;
    }
}

// Funzione di callback per il timer
function timerCallback() {
    if (timeLeft > 0) {
        timeLeft--;
        updateDisplay();
    } else {
        // Timer completato
        clearInterval(timer);
        isRunning = false;
        
        // Suona una notifica
        const audio = new Audio('https://assets.mixkit.co/sfx/preview/mixkit-alarm-digital-clock-beep-989.mp3');
        audio.play();
        
        // Cambia modalità
        if (currentMode === 'work') {
            sessionsCompleted++;
            if (sessionsCompleted % LONG_BREAK_INTERVAL === 0) {
                currentMode = 'longBreak';
                timeLeft = LONG_BREAK;
            } else {
                currentMode = 'shortBreak';
                timeLeft = SHORT_BREAK;
            }
        } else {
            currentMode = 'work';
            timeLeft = WORK_TIME;
        }
        
        updateModeLabel();
        updateDisplay();
    }
}

// Funzione per avviare il timer
function startTimer() {
    if (isRunning) return;
    
    isRunning = true;
    timer = setInterval(timerCallback, 1000);
}

// Funzione per mettere in pausa il timer
function pauseTimer() {
    if (!isRunning) return;
    
    clearInterval(timer);
    isRunning = false;
}

// Funzione per resettare il timer
function resetTimer() {
    clearInterval(timer);
    isRunning = false;
    currentMode = 'work';
    timeLeft = WORK_TIME;
    sessionsCompleted = 0;
    updateModeLabel();
    updateDisplay();
}

// Event listeners
startBtn.addEventListener('click', startTimer);
pauseBtn.addEventListener('click', pauseTimer);
resetBtn.addEventListener('click', resetTimer);

// Inizializzazione
updateModeLabel();
updateDisplay();