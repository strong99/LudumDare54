function resizePreview(id) {
    const inner = document.getElementById(id);
    const preview = inner.parentElement;
    const scaleX = preview.clientWidth / inner.clientWidth;
    const scaleY = preview.clientHeight / inner.clientHeight;

    const scale = Math.min(scaleX, scaleY);

    inner.style.transform = `scale(${scale}, ${scale})`;
}

/** @type {{[key:string]:HTMLAudioElement}} music */
const sounds = {};
function loadSound(soundFile) {
    if (!sounds[soundFile]) {
        const audio = new Audio();
        audio.src = soundFile;
        audio.preload = "auto";
        sounds[soundFile] = audio;
    }
}

/** @type {HTMLAudioElement} music */
let music;
function playMusic(soundFile) {
    if (!sounds[soundFile]) {
        const audio = new Audio();
        audio.src = soundFile;
        audio.preload = "auto";
        sounds[soundFile] = audio;
    }
    const nextMusic = sounds[soundFile];
    if (music && music != nextMusic) {
        music.pause();
        music.currentTime = 0;
    }
    music = nextMusic;
    music.volume = musicVolume * masterVolume * 0.75 * overridingMusicVolumeValue;
    music.loop = true;
    music.play();
}

function stopMusic() {
    if (music) {
        music.pause();
        music.currentTime = 0;
        music = null;
    }
}

let overridingMusicVolumeValue = 1;
let overridingMusicVolumeTimer;
let overridingMusicVolume = false;

/**
 * @param {string} soundFile
 * @param {boolean} override
 */
function playSound(soundFile, override) {
    if (!sounds[soundFile]) {
        const audio = new Audio();
        audio.src = soundFile;
        audio.preload = "auto";
        sounds[soundFile] = audio;
    }

    if (override) {
        overridingMusicVolume = true;
    }

    const sound = sounds[soundFile];
    sound.volume = soundVolume * masterVolume;
    sound.currentTime = 0;
    sound.play();

    delete sound.ended;
    if (override) {
        clearInterval(overridingMusicVolumeTimer);
        overridingMusicVolumeTimer = setInterval(() => {
            if (overridingMusicVolume) {
                overridingMusicVolumeValue = Math.max(overridingMusicVolumeValue - 0.1, 0);
            }
            else {
                overridingMusicVolumeValue = Math.min(overridingMusicVolumeValue + 0.1, 1);
                if (overridingMusicVolumeValue == 1) {
                    clearInterval(overridingMusicVolumeTimer);
                    sound.removeEventListener("ended", reEnableMusic);
                }
            }
            updateMusicVolume();
        }, 50);
        sound.addEventListener("ended", reEnableMusic);
    }
}

function reEnableMusic() {
    overridingMusicVolume = false;
}

let masterVolume = 1;
function setAudioVolume(volume) {
    masterVolume = volume;
    updateMusicVolume();
    updateSoundVolumes();
}

let soundVolume = 1;
function setSoundVolume(volume) {
    soundVolume = volume;
    updateSoundVolumes();
}

let musicVolume = 1;
function setMusicVolume(volume) {
    musicVolume = volume;
    updateMusicVolume();
}

function updateSoundVolumes() {
    for (const audio of sounds) {
        if (audio === music) {
            continue;
        }
        audio.volume = soundVolume * masterVolume;
    }
}

function updateMusicVolume() {
    if (music) {
        music.volume = musicVolume * masterVolume * 0.75 * overridingMusicVolumeValue;
    }
}