using System.Collections.Generic;
using Minis;
using UnityEngine;
using UnityEngine.InputSystem;

public class MIDIScript : MonoBehaviour {
    public List<MidiDevice> midiDevices = new List<MidiDevice>();

    Router router;

    void Start() {
        router = Router.instance;

        InputSystem.onDeviceChange += (device, change) => {
            MidiDevice midiDevice = device as MidiDevice;
            if (midiDevice == null) return;

            if (change == InputDeviceChange.Removed) {
                midiDevices.Remove(midiDevice);
            }
            else if (change != InputDeviceChange.Added) {
                return;
            }

            Debug.Log("Added MIDI device: " + midiDevice);
            midiDevices.Add(midiDevice);

            midiDevice.onWillNoteOn += (note, velocity) => {
                MidiDevice noteDevice = note.device as MidiDevice;
                if (noteDevice == null) return;

                // Note that you can't use note.velocity because the state
                // hasn't been updated yet (this is a "will" event).
                // Debug.Log(string.Format(
                //     "Note On #{0} ({1}) vel:{2:0.00} ch:{3} dev:'{4}' {5}",
                //     note.noteNumber,
                //     note.shortDisplayName,
                //     velocity,
                //     noteDevice.channel,
                //     note.device.description.product,
                //     Time.time
                // ));

                router.ReceiveEvent("note", noteDevice.channel + 1,
                    string.Format("{0}", note.noteNumber), note.noteNumber, velocity);
            };

            midiDevice.onWillNoteOff += (note) => {
                MidiDevice noteDevice = note.device as MidiDevice;
                if (noteDevice == null) return;

                // Debug.Log(string.Format(
                //     "Note Off #{0} ({1}) ch:{2} dev:'{3}'",
                //     note.noteNumber,
                //     note.shortDisplayName,
                //     noteDevice.channel,
                //     note.device.description.product
                // ));
            };

            midiDevice.onWillControlChange += (control, value) => {
                MidiDevice controlDevice = control.device as MidiDevice;
                if (controlDevice == null) return;

                // Debug.Log(string.Format(
                //     "Control Change #{0} ({1}) value:{2:0.000} ch:{3} dev:'{4}'",
                //     control.controlNumber,
                //     control.shortDisplayName,
                //     value,
                //     controlDevice.channel,
                //     control.device.description.product
                // ));

                router.ReceiveEvent("cc" + control.controlNumber, controlDevice.channel + 1,
                    string.Format("{0:0.00}", value), value, 0);
            };
        };
    }
}
