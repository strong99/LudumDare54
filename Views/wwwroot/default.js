function resizePreview(id) {
    const inner = document.getElementById(id);
    const preview = inner.parentElement;
    const scaleX = preview.clientWidth / inner.clientWidth;
    const scaleY = preview.clientHeight / inner.clientHeight;

    const scale = Math.min(scaleX, scaleY);

    inner.style.transform = `scale(${scale}, ${scale})`;
}

/** @type {{[key:string]:Audio}} music */
const sounds = {} ;
function loadSound(soundFile) {
    if (!sounds[soundFile]) {
        const audio = new Audio();
        audio.src = soundFile;
        audio.preload = "auto";
        sounds[soundFile] = audio;
    }
}

/** @type {Audio} music */
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
    music.volume = musicVolume * masterVolume;
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

function playSound(soundFile) {
    if (!sounds[soundFile]) {
        const audio = new Audio();
        audio.src = soundFile;
        audio.preload = "auto";
        sounds[soundFile] = audio;
    }
    const sound = sounds[soundFile];
    sound.volume = soundVolume * masterVolume;
    sound.currentTime = 0;
    sound.play();
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
        music.volume = musicVolume * masterVolume;
    }
}