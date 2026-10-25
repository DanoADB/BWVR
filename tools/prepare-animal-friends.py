"""Copy only the purchased gorilla from a local Unity package into ignored artwork."""
import argparse
from pathlib import Path, PurePosixPath
import tarfile

parser = argparse.ArgumentParser(description=__doc__)
parser.add_argument('package', type=Path)
args = parser.parse_args()
destination = Path(__file__).resolve().parents[1] / 'Assets/LocalLicensedArt/AnimalFriends/Gorilla'
prefix = 'Assets/Wild Animals/Gorilla/'
count = 0
with tarfile.open(args.package, 'r:gz') as archive:
    members = {m.name: m for m in archive.getmembers() if m.isfile()}
    for name, member in members.items():
        if not name.endswith('/pathname'):
            continue
        source = archive.extractfile(member).read().decode('utf-8-sig').splitlines()[0].rstrip('\0')
        if not source.startswith(prefix):
            continue
        relative = PurePosixPath(source[len(prefix):])
        if relative.is_absolute() or '..' in relative.parts:
            raise ValueError('Unsafe package path')
        target = destination.joinpath(*relative.parts)
        if not target.resolve().is_relative_to(destination.resolve()):
            raise ValueError('Package path escapes destination')
        folder = name.rsplit('/', 1)[0]
        asset = members.get(folder + '/asset')
        if asset is None:
            target.mkdir(parents=True, exist_ok=True)
        else:
            target.parent.mkdir(parents=True, exist_ok=True)
            target.write_bytes(archive.extractfile(asset).read())
            count += 1
        metadata = members.get(folder + '/asset.meta')
        if metadata is not None:
            Path(str(target) + '.meta').write_bytes(archive.extractfile(metadata).read())
if count == 0:
    raise SystemExit('No Animal Friends gorilla files found; check the purchased package.')
print(f'Prepared {count} gorilla files in {destination}. Licensed artwork remains ignored by Git.')
