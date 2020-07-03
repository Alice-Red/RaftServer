/**
 * Copyright © 2018 Redkun./teito.
 * Copyright © AliceRed. All rights reserved.
 * 
 */

enchant();
IsDebug = false;
FPS = 0;
AutoRegeneration = 10;

cursor = null;
player = null;
CHSH = null;
Hit = null;
PLbullets = null;
Notes = null;
BBullets = null;
Effects = null;
wave = null;

PLbulletsID = 0;
NotesID = 0;
BBulletsID = 0;
EffectsID = 0;

Score = 0;
KillCount = 0;

BossAlive = null;

// IsPause = false;
// IsPauseR = false;

Resources = {
    c2: "resource/c2.png",
    ef: "resource/testEF.png",
    ns: "resource/notes.png",
    nt: "resource/note.png",
    pl: "resource/jiki.png",
    pb: "resource/jikib.png",
    bl: "resource/bullet.png",
    db: "resource/debugP.png",
    la: "resource/yes_Laser.wav",
    la11: "resource/Laser_Shoot11.wav",
    la26: "resource/Laser_Shoot26.wav",
    la04: "resource/Hit_Hurt4.wav",
    ex: "resource/Hit_Hurt32.wav",
    gr: "resource/Hit_Hurt16.wav",
    ph: "resource/Explosion11.wav",
    pc: "resource/phc.png",
    bb: "resource/bb.png",
    fl: "resource/fill.png",
    ch: "resource/charges.png",
    bs: "resource/boss.png",
    bf: "resource/Explosion18.wav",

};

SPAWN = {
    Right: 0,
    Top: 1,
    Left: 2,
    Bottom: 3,
};

function ToPlayerLiner(obj, speed) {
    var t = Math.pow(Hit.x - obj.x, 2) + Math.pow(Hit.y - obj.y, 2);
    t = Math.sqrt(t) / speed;
    obj.tl.moveBy(Hit.x - obj.x, Hit.y - obj.y, Math.floor(t)).loop();
}

// function ToPlayerEasing(obj, speed) {
//     var t = Math.pow(Hit.x - obj.x, 2) + Math.pow(Hit.y - obj.y, 2);
//     t = Math.sqrt(t) / speed;
//     obj.tl.moveTo(Hit.x - obj.x, Hit.y - obj.y, Math.floor(t));
// }

function ToLiner(obj, vx, vy, speed) {
    var t = getDistance(obj.x, obj.y, obj.x + vx, obj.y + vy) / speed;
    obj.tl.moveBy(vx, vy, Math.floor(t)).loop();
}

Barrages = {
    wave1: {
        ID: 1,
        Type: 1,
        loop: function () {
            return Score < 3000 && KillCount <= 15;
        },
        spawn: [
            SPAWN.Right,
        ],
        rate: 60,
        // speed:
        elements: [{
            reteW: 180,
            rateF: 12,
            cutW: 3,
            atkB: 50,
            atkE: 10,
            speed: 4,
            hp: 70,
            move: function (obj) {
                ToPlayerLiner(obj, 2);
            },
            moveBurrets: function (obj) {
                ToPlayerLiner(obj, 4);
            },
        },],
    },
    wave2: {
        ID: 2,
        Type: 1,
        loop: function () {
            return Score < 8500 && KillCount <= 40;
        },
        rate: 60,
        spawn: [
            SPAWN.Right,
            SPAWN.Left,
        ],
        elements: [{
            reteW: 120,
            rateF: 20,
            cutW: 5,
            atkB: 60,
            atkE: 20,
            speed: 4,
            hp: 170,
            move: function (obj) {
                ToPlayerLiner(obj, 2);
            },
            moveBurrets: function (obj) {
                ToPlayerLiner(obj, 4);
            },
        },],
    },
    wave3: {
        ID: 3,
        Type: 1,
        loop: function () {
            return Score <= 15000 && KillCount <= 70;
        },
        rate: 60,
        spawn: [
            SPAWN.Top,
            SPAWN.Bottom,
        ],
        elements: [{
            reteW: 150,
            rateF: 6,
            cutW: 4,
            atkB: 50,
            atkE: 20,
            speed: 3,
            hp: 400,
            move: function (obj) {
                ToPlayerLiner(obj, 1);
            },
            moveBurrets: function (obj) {
                ToPlayerLiner(obj, 3);
            },
        },],

    },
    wave4: {
        ID: 4,
        Type: 1,
        loop: function () {
            return Score <= 25000 && KillCount <= 130;
        },
        rate: 40,
        spawn: [
            SPAWN.Right,
            SPAWN.Left,
            SPAWN.Top,
        ],
        elements: [{
            reteW: 180,
            rateF: 2,
            cutW: 6,
            atkB: 60,
            atkE: 20,
            speed: 5,
            hp: 90,
            move: function (obj) {
                ToPlayerLiner(obj, 2);
            },
            moveBurrets: function (obj) {
                ToPlayerLiner(obj, 5);
            },
        },],

    },
    wave5: {
        ID: 5,
        Type: 1,
        loop: function () {
            return Score < 40000 && KillCount <= 200;
        },
        rate: 20,
        spawn: [
            SPAWN.Right,
            SPAWN.Left,
            SPAWN.Top,
            SPAWN.Bottom,
        ],
        elements: [{
            reteW: 100,
            rateF: 25,
            cutW: 5,
            atkB: 60,
            atkE: 20,
            speed: 6,
            hp: 90,
            move: function (obj) {
                ToPlayerLiner(obj, 2);
            },
            moveBurrets: function (obj) {
                ToPlayerLiner(obj, 6);
            },
        },],

    },
    waveB: {
        ID: "Boss",
        Type: 2,
        loop: function () {
            return BossAlive == null || BossAlive.HP > -700;
        },
        spawn: [],

    },
    wave6: {
        ID: "Bonus",
        Type: 1,
        loop: function () {
            return true;
        },
        rate: 5,
        spawn: [
            SPAWN.Right,
            SPAWN.Left,
            SPAWN.Top,
            SPAWN.Bottom,
        ],
        elements: [{
            reteW: 12,
            rateF: 12,
            cutW: 5,
            atkB: 60,
            atkE: 60,
            speed: 12,
            hp: 90,
            move: function (obj) {
                ToPlayerLiner(obj, 8);
            },
            moveBurrets: function (obj) {
                ToPlayerLiner(obj, 6);
            },
        },],

    },
};

function RAND(a, b) {
    if (a > b) {
        let t = a;
        a = b;
        b = t;
    }
    return Math.floor(Math.random() * (Math.floor(b - a) + 1) + a);
}

function getDistance(x1, y1, x2, y2) {
    var distance = Math.sqrt((x2 - x1) * (x2 - x1) + (y2 - y1) * (y2 - y1));
    return distance;
}

function getDegree(x1, y1, x2, y2) {
    var radian = Math.atan2(y2 - y1, x2 - x1);
    var degree = radian * (180 / Math.PI);
    return degree;
}

function getRange(degree, radius) {
    var radian = degree * Math.PI / 180;
    y2 = Math.sin(radian) * radius;
    x2 = Math.cos(radian) * radius;
    return {
        x: x2 * -1,
        y: y2
    };
}

function NomalizeDeg(d, k) {
    while (d < 0)
        d += k;
    while (d >= k)
        d -= k;
    return d;
}

function InField(x, y, m) {
    return (x >= (0 - m) &&
        x <= (1280 + m) &&
        y >= (0 - m) &&
        y <= (720 + m));
}

Cursor = Class.create(Sprite, {
    initialize: function (x, y) {
        Sprite.call(this, 32, 32);
        this.image = game.assets[Resources.c2];
        this.x = x;
        this.y = y;
        this.OnClicked = false;
        this.firingRate = 8;
        this.CoolDown = 0;
        this._style.zIndex = 4;

        game.rootScene.addChild(this);
    },
    // touchEnabled: false
    ontouchstart: function () {
        this.OnClicked = true;
        this.Fire();
    },
    ontouchmove: function () {
        this.OnClicked = true;
    },
    ontouchend: function () {
        this.CoolDown = 0;
        this.OnClicked = false;
    },
    onenterframe: function () {
        this.tx = this.x + 16;
        this.ty = this.y + 16;
        if (this.CoolDown <= 0) {
            if (this.OnClicked) {
                this.Fire();
                CHSH.Chargeable = false;
            } else if (game.input.space) {
                CHSH.Chargeable = true;
            } else {
                CHSH.Chargeable = false;
            }
        } else {
            this.CoolDown -= 1;
            // if (!game.input.shift)
            //     this.CoolDown -= 1;
        }
    },
    Fire: function () {
        bullet = new Bullet();
        this.CoolDown = this.firingRate;
    },


});

Charging = Class.create(Sprite, {

    initialize: function () {
        Sprite.call(this, 128, 128);
        this.charge = 0;
        this.chargeMax = 399;
        this.Fired = false;
        this.Fill = 340;
        this.opacity = 0;
        this.touchEnabled = false;
        this.Chargeable = false;
        this.FillE = false;
        this.image = game.assets[Resources.ch];
        this.rt = RAND(0, 359);
        game.rootScene.addChild(this);
    },
    onenterframe: function () {
        this.x = player.tx - 64;
        this.y = player.ty - 64;
        this.rotation = this.rt + NomalizeDeg(-player.frame, 360);
        if (!this.Fired) {
            if (this.Chargeable) {
                if (this.opacity < 1)
                    this.opacity += 0.1;
                this.charge += 5.5;
                if (game.input.shift)
                    this.charge += 3.8;
                if (this.charge >= this.Fill && !this.FillE) {
                    fe = new FillEffect();
                    this.FillE = true;
                }
                if (this.charge > this.chargeMax) {
                    this.charge = this.chargeMax;
                }
            } else {
                if (this.charge < this.Fill) {
                    if (this.opacity > 0)
                        this.opacity -= 0.2;
                    this.charge -= 5;
                    if (this.charge < 0)
                        this.charge = 0;

                } else {
                    this.Fired = true;
                }
            }
            // this.frame = Math.floor(this.charge);
        } else {
            if (this.charge >= this.Fill) {
                ChBull = new ChargeShot();
                this.charge = this.Fill
                this.FillE = false;
            }
            this.opacity -= 0.1;
            this.charge -= 2;
            if (this.opacity <= 0) {
                this.charge = 0;
                this.opacity = 0;
                this.Fired = false;
            }
        }
        this.frame = Math.floor(this.charge);

    },

});

FillEffect = Class.create(Sprite, {
    initialize: function () {
        Sprite.call(this, 128, 128);

        this.x = player.tx - 32;
        this.y = player.ty - 32;

        this.scaleX = 0.1;
        this.scaleY = 0.1;
        this.opacity = 1;
        this.touchEnabled = false;
        this.image = game.assets[Resources.fl];
        game.rootScene.addChild(this);
    },
    onenterframe: function () {
        this.scaleX += 0.2;
        this.scaleY += 0.2;
        if (this.scaleX > 1) {
            this.opacity -= 0.05;
            if (this.opacity <= 0)
                game.rootScene.removeChild(this);
        }
    },

});

ChargeShot = Class.create(Sprite, {
    initialize: function () {
        Sprite.call(this, 4, 26);
        this.image = game.assets[Resources.bl];
        this.x = player.tx - 2;
        this.y = player.ty - 13;
        this.scaleX = 12;
        this.scaleY = 4;
        this.ATK = 200;
        this.speed = 30;
        this._style.zIndex = 1;
        this.touchEnabled = false;
        this.rotation = NomalizeDeg((getDegree(player.tx, player.ty, cursor.tx, cursor.ty) + 90), 360);
        this.s = getRange(this.rotation * -1 - 90, this.speed);

        game.rootScene.addChild(this);

        let sound = game.assets[Resources.la26].clone();
        sound.play();

    },
    onenterframe: function () {
        this.x += this.s.x;
        this.y += this.s.y;

        if (!InField(this.x, this.y, 128)) {
            this.DeleteObject();
        }
        for (let i = 0; i < Notes.length; i++) {
            if (this.within(Notes[i], 50)) {
                Notes[i].HP -= this.ATK;
            }
        }

        for (let i = 0; i < BBullets.length; i++) {
            if (this.within(BBullets[i], 50)) {
                Score += 10;
                let sound = game.assets[Resources.gr].clone();
                sound.play();
                BBullets[i].DeleteObject();
            }
        }

    },
    DeleteObject: function () {
        game.rootScene.removeChild(this);
    },

});

Note = Class.create(Sprite, {
    initialize: function (x, y, m) {
        Sprite.call(this, 64, 64);
        this.ID = NotesID++;
        this.image = game.assets[Resources.nt];
        this.elements = [];
        this.x = x;
        this.y = y;
        this.HP = m.hp;
        this.ATK = m.atkE;
        this.ATKB = m.atkB;
        this.frame = RAND(0, 89);
        this.touchEnabled = false;
        this.Move = m.move;
        this.MoveB = m.moveBurrets;

        this.elCount = m.cutW;
        this.elCountPP = 0;
        this.WaveRate = m.reteW;
        this.WaveCount = 0;
        this.FiringRate = m.rateF;
        this.CoolDown = 0;
        this.speedB = m.speed;

        this._style.zIndex = 2;
        game.rootScene.addChild(this);
        Notes.push(this);
        this.Move(this);
    },
    onenterframe: function () {
        for (let i = 0; i < PLbullets.length; i++) {
            if (this.within(PLbullets[i], 28)) {
                this.HP -= PLbullets[i].ATK;
                PLbullets[i].DeleteObject();
            }
        }

        for (let i = 0; i < Effects.length; i++) {
            if (this.within(Effects[i], 40)) {
                this.HP -= Effects[i].ATK;
            }
        }
        if (!InField(this.x, this.y, 128))
            this.DeleteObject();
        this.frame = NomalizeDeg(this.frame + 3, 90);
        if (this.HP <= 0) {
            effect = new Effect(this.x + 32, this.y + 32);
            let sound = game.assets[Resources.ex].clone();
            sound.play();
            Score += 100;
            KillCount += 1;
            this.DeleteObject();
        }

        if (this.WaveCount <= 0)
            this.elCountPP = 0;
        if (this.elCountPP < this.elCount && this.CoolDown <= 0) {
            this.elements[++this.elCountPP] = new BulletE(this.x + 32, this.y + 32, this.ATKB, {
                x: Hit.x + 4,
                y: Hit.y + 4
            }, this.MoveB);
            this.CoolDown = this.FiringRate;
            this.WaveCount = this.WaveRate;
        }
        this.CoolDown -= 1;
        this.WaveCount -= 1;
    },
    DeleteObject: function () {
        game.rootScene.removeChild(this);
        for (let i = 0; i < Notes.length; i++) {
            if (Notes[i].ID == this.ID) {
                Notes.splice(i, 1);
            }
        }
    },

});

Boss = Class.create(Sprite, {
    initialize: function () {
        Sprite.call(this, 64, 64);
        this.touchEnabled = false;
        this.image = game.assets[Resources.bs];
        this.HP = 16000;
        this.SafeRound = 10;
        this.x = 1280 / 2;
        this.y = 50;
        this.velR = 180;
        this.iR = 0;
        this.rest = 20;
        this.rush = -1;
        this.touchEnabled = false;
        this.wave = 0;
        game.rootScene.addChild(this);
    },
    onenterframe: function () {
        if (this.HP > 0) {

            this.frame = NomalizeDeg(this.frame + 1, 90);
            if (this.age % 3 == 0) {
                let of = 0;
                let ttv = null;
                let tvR = null;
                while (ttv == null || !InField(this.tx + ttv.x, this.ty + ttv.y, -40)) {
                    tvR = RAND(this.velR - (this.SafeRound + of), this.velR + (this.SafeRound + of));
                    ttv = getRange(tvR, 5);
                    of += 5;
                }
                this.velR = tvR;
            }

            let tv = getRange(this.velR, 1);

            this.x += tv.x;
            this.y += tv.y;
            this.tx = this.x + 16;
            this.ty = this.y + 16;

            switch (this.wave) {
                case 0:

                    break;
                case 1:
                    if (this.age % 8 == 0) {
                        this.iR = NomalizeDeg(this.iR + 3, 360);
                        for (let i = 0; i < 360; i += 15) {
                            let t = getRange(i + this.iR, 32);
                            let m = function (obj) {
                                ToLiner(obj, t.x, t.y, 5);
                            }
                            let p = {
                                x: this.tx + (t.x * 2),
                                y: this.ty + (t.y * 2)
                            }
                            let tmp = new BulletE(this.tx + t.x, this.ty + t.y, 120, p, m)
                        }
                        let sound = game.assets[Resources.bf].clone();
                        sound.play();
                    }
                    break;
                case 2:
                    if (this.age % 24 == 0) {
                        this.iR = NomalizeDeg(this.iR + 2.5, 360);
                        for (let i = 0; i < 360; i += 5) {
                            let t = getRange(i + this.iR, 32);
                            let m = function (obj) {
                                ToLiner(obj, t.x, t.y, 4);
                            }
                            let p = {
                                x: this.tx + (t.x * 2),
                                y: this.ty + (t.y * 2)
                            }
                            let tmp = new BulletE(this.tx + t.x, this.ty + t.y, 140, p, m)
                        }
                        let sound = game.assets[Resources.bf].clone();
                        sound.play();
                    }
                    break;
                case 3:
                    if (this.age % 6 == 0) {
                        this.iR = NomalizeDeg(this.iR + 15, 360);
                        for (let i = 0; i < 360; i += 30) {
                            let t = getRange(i + this.iR, 32);
                            let m = function (obj) {
                                ToLiner(obj, t.x, t.y, 4);
                            }
                            let p = {
                                x: this.tx + (t.x * 2),
                                y: this.ty + (t.y * 2)
                            }
                            let tmp = new BulletE(this.tx + t.x, this.ty + t.y, 100, p, m)
                        }
                        let sound = game.assets[Resources.bf].clone();
                        sound.play();
                    }
                    break;
                case 4:
                    if (this.age % 8 == 0) {
                        this.iR = NomalizeDeg(this.iR + RAND(1, 89), 360);
                        for (let i = 0; i < 360; i += 18) {
                            let t = getRange(i + this.iR, 32);
                            let m = function (obj) {
                                ToLiner(obj, t.x, t.y, 2);
                            }
                            let p = {
                                x: this.tx + (t.x * 5),
                                y: this.ty + (t.y * 5)
                            }
                            let tmp = new BulletE(this.tx + t.x, this.ty + t.y, 100, p, m)
                        }
                        let sound = game.assets[Resources.bf].clone();
                        sound.play();
                    }
                    break;
            }


            if (this.rush == 0) {
                this.wave = 0;
                this.rest = 60;
            }
            if (this.rest == 0) {
                this.wave = RAND(1, 4);
                this.rush = 1200;
            }
            this.rush -= 1;
            this.rest -= 1;


            for (let i = 0; i < PLbullets.length; i++) {
                if (this.within(PLbullets[i], 28)) {
                    this.HP -= PLbullets[i].ATK;
                    PLbullets[i].DeleteObject();
                }
            }
        } else {
            if (this.opacity > 0)
                this.opacity -= 0.5;
            if (this.age % 15 == 0) {
                if (this.HP > -1000) {
                    this.HP -= 150;
                    effect = new Effect(this.tx + RAND(-32, 32), this.ty + RAND(-32, 32));
                    let sound = game.assets[Resources.ex].clone();
                    sound.play();
                } else {
                    Score += 5000;
                    game.rootScene.removeChild(this);
                }
            }
        }

    },
});



BulletE = Class.create(Sprite, {
    initialize: function (x, y, a, p, m) {
        Sprite.call(this, 16, 26);
        this.ID = BBulletsID++;
        this.image = game.assets[Resources.bb];
        this.x = x;
        this.y = y;
        this.tx = this.x + 8;
        this.ty = this.y + 13;
        this.ATK = a;
        this.Move = m;
        // this.speed = s;
        this.touchEnabled = false;
        this.rotation = (getDegree(this.tx, this.ty, p.x, p.y) + 90);
        // this.param = getRange(-(getDegree(this.tx, this.ty, p.x, p.y) + 180), this.speed);
        game.rootScene.addChild(this);
        this.Move(this);
        // let sound = game.assets[Resources.].clone();
        // sound.play();
        BBullets.push(this);
    },
    onenterframe: function () {
        // this.x += this.param.x;
        // this.y += this.param.y;
        this.tx = this.x + 8;
        this.ty = this.y + 13;
        if (this.within(player, 32) && this.age % 8 == 0) {
            Score += 10;
            let sound = game.assets[Resources.gr].clone();
            sound.play();
        }
        if (!InField(this.x, this.y, 128)) {
            this.DeleteObject();
        }

    },
    DeleteObject: function () {
        game.rootScene.removeChild(this);
        for (let i = 0; i < BBullets.length; i++) {
            if (BBullets[i].ID == this.ID) {
                BBullets.splice(i, 1);
            }
        }
    },
});

DebugP = Class.create(Sprite, {
    initialize: function () {
        Sprite.call(this, 32, 32);
        this.touchEnabled = false;
        this.image = game.assets[Resources.db];
        game.rootScene.addChild(this);
    },
    onenterframe: function () {
        this.x = player.tx;
        this.y = player.ty;
    },

});

Effect = Class.create(Sprite, {
    initialize: function (x, y) {
        Sprite.call(this, 256, 256);
        this.image = game.assets[Resources.ef];
        this.ID = EffectsID++;
        this.x = x - 128;
        this.y = y - 128;
        this.ATK = 0.4;
        this.rotation = RAND(0, 360);
        this.frame = 0;
        this.touchEnabled = false;
        game.rootScene.addChild(this);
        Effects.push(this);
    },
    onenterframe: function () {
        this.frame += 1;
        if (this.frame == 64) {
            this.DeleteObject();
        }
    },
    DeleteObject: function () {
        game.rootScene.removeChild(this);
        for (let i = 0; i < Effects.length; i++) {
            if (Effects[i].ID == this.ID) {
                Effects.splice(i, 1);
            }
        }
    },

});

Bullet = Class.create(Sprite, {
    initialize: function () {
        Sprite.call(this, 4, 26);
        this.image = game.assets[Resources.bl];
        this.ID = PLbulletsID++;
        this.x = player.tx - 2;
        this.y = player.ty - 13;
        this.ATK = 80;
        this.speed = 8;
        this.speedS = 16;
        this._style.zIndex = 1;
        this.touchEnabled = false;
        this.rotation = NomalizeDeg((getDegree(player.tx, player.ty, cursor.tx, cursor.ty) + 90), 360);
        this.s = getRange(this.rotation * -1 - 90, (game.input.shift) ? this.speedS : this.speed);

        game.rootScene.addChild(this);
        PLbullets.push(this);


        if (game.input.shift) {
            let sound = game.assets[Resources.la04].clone();
            sound.play();
        } else {
            let sound = game.assets[Resources.la].clone();
            sound.play();
        }

    },
    onenterframe: function () {
        this.x += this.s.x;
        this.y += this.s.y;

        if (!InField(this.x, this.y, 128)) {
            this.DeleteObject();
        }
    },
    DeleteObject: function () {
        game.rootScene.removeChild(this);
        for (let i = 0; i < PLbullets.length; i++) {
            if (PLbullets[i].ID == this.ID) {
                PLbullets.splice(i, 1);
            }
        }
    },

});

Player = Class.create(Sprite, {
    initialize: function () {
        Sprite.call(this, 64, 64);
        this.x = 100;
        this.y = (720 / 2) - 32;
        this.MAXHP = 8000;
        this.Star = 0;
        this.HP = this.MAXHP;
        this._style.zIndex = 4;
        this.touchEnabled = false;
        this.image = game.assets[Resources.pb];
        game.rootScene.addChild(this);
        CHSH = new Charging();
        Hit = new HitCircleP();
        // DBP = new DebugP();
    },
    onenterframe: function () {
        this.frame = NomalizeDeg(-(getDegree(this.x, this.y, cursor.x - 16, cursor.y - 16) + 90), 360);

        if (this.HP <= 0) {
            var GameoverScene = new Scene();
            GameoverScene.backgroundColor = '#000000';

            var labelOver = new Label();
            labelOver.font = "32px consolas";
            labelOver.x = 100;
            labelOver.y = 200;
            labelOver.color = '#FFFFFF';
            labelOver.text = "Score: " + Score;
            GameoverScene.addChild(labelOver);

            game.pushScene(GameoverScene);
            game.stop();
        }
        this.HP += (AutoRegeneration / 60);
        if (this.HP > this.MAXHP)
            this.HP = this.MAXHP;
        if (this.Star > 0)
            this.Star -= 1;

        var sp = 3;
        if (game.input.shift) {
            sp /= 2;
        }
        if (game.input.space) {
            sp = (sp * 2) / 3;
        }

        if (game.input.u) {
            this.y -= sp;
        }
        if (game.input.d) {
            this.y += sp;
        }
        if (game.input.l) {
            this.x -= sp;
        }
        if (game.input.r) {
            this.x += sp;
        }

        if (this.y <= 0)
            this.y = 0;
        if (this.x <= 0)
            this.x = 0;
        if (this.y >= 720 - 64)
            this.y = 720 - 64;
        if (this.x >= 1280 - 64)
            this.x = 1280 - 64;

        Hit.x = this.x + 24;
        Hit.y = this.y + 24;
        var tmp = getRange(this.frame - 90, 32);
        this.tx = this.x + 32 + tmp.x;
        this.ty = this.y + 32 + tmp.y;
    },
});

HitCircleP = Class.create(Sprite, {
    initialize: function () {
        Sprite.call(this, 16, 16);
        this.touchEnabled = false;
        this.image = game.assets[Resources.pc];
        this.opacity = 0;
        game.rootScene.addChild(this);
    },
    onenterframe: function () {
        if (game.input.shift)
            this.opacity = 1;
        else
            this.opacity = 0;

        this.x = player.x + 24;
        this.y = player.y + 24;
        this.tx = this.x + 2;
        this.ty = this.y + 4;

        if (player.Star == 0) {
            for (let i = 0; i < BBullets.length; i++) {
                if (this.within(BBullets[i], 8)) {
                    player.HP -= BBullets[i].ATK;
                    let sound = game.assets[Resources.ph].clone();
                    player.Star = 4;
                    sound.play();
                    BBullets[i].DeleteObject();
                }
            }
            for (let i = 0; i < Notes.length; i++) {
                if (this.within(Notes[i], 10)) {
                    player.HP -= Notes[i].ATK;
                    let sound = game.assets[Resources.ph].clone();
                    player.Star = 2;
                    sound.play();
                }
            }
        }

    },
});

window.onload = function () {
    game = new Game(1280, 720);
    // game = new Game(1920, 1080);
    for (item in Resources) {
        game.preload(Resources[item]);
    }
    game.fps = 59;
    game.keybind('W'.charCodeAt(0), 'u');
    game.keybind('A'.charCodeAt(0), 'l');
    game.keybind('S'.charCodeAt(0), 'd');
    game.keybind('D'.charCodeAt(0), 'r');
    game.keybind(16, 'shift');
    game.keybind(32, 'space');
    game.keybind(18, 'alt');


    document.onkeydown = function (e) {
        e = e || window.event;
        if (e.ctrlKey) {
            var c = e.which || e.keyCode;
            switch (c) {
                case 87: // Ctrl + W --Not work in Chrome
                case 65: // ctrl + A
                case 83: // Ctrl + S
                case 68: // ctrl + D
                case 69: // ctrl + E
                    e.preventDefault();
                    e.stopPropagation();
                    break;
                default:
                    console.log(c);
                    break;
            }
        }
    };

    game.onload = function () {
        game.rootScene.backgroundColor = "white";

        player = new Player();
        cursor = new Cursor(0, 0);
        PLbullets = [];
        Notes = [];
        BBullets = [];
        Effects = [];

        var label = [];
        for (let i = 1; i <= 4; i++) {
            label[i] = new Label();
            label[i].font = "12px consolas";
            label[i].x = 3;
            label[i].y = 3 + (12 * (i - 1));
            game.rootScene.addChild(label[i]);
        }

        var newTime = null;
        var pastTime = null;
        var GAge = 0;
        var Resumer = function () {
            game.resume();
        }

        // 毎フレーム描画処理
        game.addEventListener('enterframe', function () {
            newTime = new Date();
            if (pastTime != null) {
                // if (pastTime - newTime < (1000 / 60)) {
                //     game.pause();
                //     // game.resume();
                //     // setTimeout(Resumer, (1000 / 60) - (pastTime - newTime));
                // }
                // newTime = new Date();
                let tmp = newTime - pastTime;
                FPS = 1000 / tmp;
            }
            pastTime = newTime;
            GAge += 1;

            setPosition();

            NGenerate();

            // if (game.input.alt) {
            //     if (!IsPause) {
            //         if (!IsPauseR) {
            //             game.pause();
            //             IsPause = true;
            //             IsPauseR = true;
            //         }
            //     }
            // }
            // IsPauseR = false;

            label[1].text = "Score: " + Score;
            label[2].text = "HP: " + Math.floor(player.HP);
            if (wave != null)
                label[3].text = "Difficulty: " + wave.ID;
            if (GAge % 10 == 0)
                label[4].text = "FPS: " + Math.floor(FPS);

            if ((PLbullets.length == 0 || PLbulletsID >= 200) && PLbulletsID != 0)
                PLbulletsID = 0;
            if ((Notes.length == 0 || NotesID >= 2000) && NotesID != 0)
                NotesID = 0;
            if ((BBullets.length == 0 || BBulletsID >= 5000) && BBulletsID != 0)
                BBulletsID = 0;
            if ((Effects.length == 0 || EffectsID >= 500) && EffectsID != 0)
                EffectsID = 0;


        });

        function setPosition() {
            window.document.onmousemove = function (e) {
                cursor.moveTo(getMousePosition(e).x, getMousePosition(e).y);
            };
        }
        function getMousePosition(e) {
            var obj = [];
            if (e) {
                obj.x = e.pageX - (cursor.width - 7); // game.scale; // - ((cursor.width * game.scale) / 2);
                obj.y = e.pageY - (cursor.height - 7); // game.scale; // - ((cursor.height * game.scale) / 2);
            } else {
                obj.x = (event.x + document.body.scrollLeft) / game.scale;
                obj.y = (event.y + document.body.scrollTop) / game.scale;
            }
            return obj;
        }

        function NGenerate() {
            for (w in Barrages) {
                if (Barrages[w].loop()) {
                    wave = Barrages[w];
                    break;
                }
            }


            if (wave != null) {
                switch (wave.Type) {
                    case 1:
                        if (RAND(1, wave.rate) == 1) {
                            let cx = 0;
                            let cy = 0;
                            let sp = RAND(1, wave.spawn.length) - 1;
                            switch (wave.spawn[sp]) {
                                case SPAWN.Right:
                                    cx = 1280 + 32;
                                    cy = RAND(50, 670);
                                    break;
                                case SPAWN.Top:
                                    cx = RAND(50, 1230);
                                    cy = -32;
                                    break;
                                case SPAWN.Left:
                                    cx = -32;
                                    cy = RAND(50, 670);
                                    break;
                                case SPAWN.Bottom:
                                    cx = RAND(50, 1230);
                                    cy = 720 + 32;
                                    break;
                                default:
                                    console.log(sp);
                                    break;
                            }
                            let elm = RAND(1, wave.elements.length) - 1;

                            note = new Note(cx, cy, wave.elements[elm]);
                        }
                        break;
                    case 2:
                        if (BossAlive == null) {
                            BossAlive = new Boss();
                        }
                        break;
                }

            }

        }


    };
    if (IsDebug)
        game.debug();
    else
        game.start();
}
