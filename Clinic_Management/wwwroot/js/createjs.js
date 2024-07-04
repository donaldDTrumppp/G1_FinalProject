function chooseAppointment(e) {
    var id = e.querySelector('[data-hs-combo-box-output-item-field]').innerHTML;

    fetch(`/api/MedicalRecords/appointments/${id}`)
        .then(response => {
            if (!response.ok) {
                throw new Error('Network response was not ok');
            }
            return response.json();
        })
        .then(data => {
            document.getElementById('floating_fname_d').value = (data.doctorName);
            document.getElementById('floating_visit_time').value = (data.requestTime);
            document.getElementById('floating_specialization').value = (data.doctorSpecialization);
            document.getElementById('floating_fname').value = (data.patientName);
            document.getElementById('floating_address').value = (data.patientAddress);
            document.getElementById('floating_dob').value = isoToInputDate(data.patientDob);
            document.getElementById('floating_phone').value = (data.patientPhone);
            document.getElementById('floating_email').value = (data.patientEmail);
            document.getElementById('apmId').value = decodeId(data.id);
            document.getElementById('patientId').value = (data.patientId);
            document.getElementById('doctorId').value = (data.doctorId);
        })
        .catch(error => {
            console.error('Error:', error);
        });
}

function isoToInputDate(isoDate) {
    var date = new Date(isoDate);
    var year = date.getFullYear();
    var month = (date.getMonth() + 1).toString().padStart(2, '0'); // Month is zero-based
    var day = date.getDate().toString().padStart(2, '0');
    return `${year}-${month}-${day}`;
}

const allowedChars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
const maxLength = 5; // Max length of the encoded string

function decode(code) {
    let number = 0;

    for (let i = 0; i < code.length; i++) {
        let charIndex = allowedChars.indexOf(code[i]);
        if (charIndex === -1) {
            throw new Error("Invalid character in code");
        }
        number = number * allowedChars.length + charIndex;
    }

    return number;
}

function decodeId(code) {
    const parts = code.split('-');
    if (parts.length !== 3) {
        throw new Error("Invalid code format");
    }

    const specializationPart = parts[2];
    const datePart = parts[1];
    const idPart = parts[0];

    // Parse the ID part
    const id = decode(idPart);

    // Return the decoded appointment details
    return id;
}

var glid;

function bfdelete(e) {
    glid = e.id;
}

function DeleteReport() {
    fetch('/api/MedicalRecords/' + glid, {
        method: 'DELETE',
    }).then((json) => {
        document.getElementById('root-' + glid).style.display = 'none';
    }).catch(error => error);
}
