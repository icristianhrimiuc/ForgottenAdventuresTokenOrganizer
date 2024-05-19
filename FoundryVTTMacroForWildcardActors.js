main();
async function main() {
    let modified = [];
    let actors = game.actors.filter(a => a.type === 'npc' && a.folder.folder != null && a.folder.folder.name == 'Monsters (SRD)');
    actors.forEach(actor => {
        if (actor.prototypeToken.texture.src != null && actor.prototypeToken.texture.src.startsWith("systems/dnd5e/tokens/")) {
            let matches = actor.prototypeToken.texture.src.match("systems\/dnd5e\/tokens\/([a-zA-Z0-9_ ]*)\/([a-zA-Z0-9_]*)\.webp");
            let newPath = "my%20data/topdown%20tokens/Forgotten%20Adventures/Creatures/" + matches[1].charAt(0).toUpperCase() + matches[1].slice(1) + '/' + actor.name.replace(" ", "%20") + '/*.png';
            game.actors.updateAll(a => (
                {
                    'prototypeToken.texture.src': newPath,
                    'prototypeToken.randomImg': true,
                    'prototypeToken.displayName': 20,
                    'prototypeToken.displayBars': 40,
                    'prototypeToken.bar1.attribute': "attributes.hp"
                }), a => a._id == actor._id);
            modified.push(actor);
        }
    });
    console.log(modified.length);
}