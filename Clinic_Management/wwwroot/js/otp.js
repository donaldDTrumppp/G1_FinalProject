window.onload = function () {
    render();

}

document.addEventListener('DOMContentLoaded', (event) => {
    var toast = document.getElementById("toast-success");
    if (toast.classList.contains("block")) {
        document.getElementById('animateDiv').classList.add("reduceWidth");
        document.getElementById('animateDiv').style.width = '0%';


    }

    function onTransitionEnd(event) {
        if (event.propertyName === 'width' && animateDiv.style.width === '0%') {
            document.getElementById('animateDiv').classList.remove("reduceWidth");
            document.getElementById('animateDiv').style.width = '100%';
            console.log(document.getElementById('animateDiv').style.width);
            //   toast.classList.add("animate-[fade-out_1s_ease-out_0.25s_1]");

        }
    }



    animateDiv.addEventListener('transitionend', onTransitionEnd);
});

function render() {
    window.recaptchaVerifier = new firebase.auth.RecaptchaVerifier("recaptcha-container", {

    });
    recaptchaVerifier.render();
}

var coderesult;

function swtch2() {
    document.getElementById("notverify").style.display = "none";
    document.getElementById("verify").style.display = "block";
    timeInMinutes = 3;
    timeInSeconds = timeInMinutes * 60;
    document.getElementById("countdown").innerHTML = "03:00";
    const countdownInterval = setInterval(updateCountdown, 1000);
    document.getElementById("phonenumber").innerHTML = a;
}

function swtch() {
    clearInterval(countdownInterval);
    document.getElementById("notverify").style.display = "block";
    document.getElementById("verify").style.display = "none";
    window.scrollTo({
        top: 0,
        behavior: 'smooth'
    });
}

let timeInMinutes = 3;
let timeInSeconds = timeInMinutes * 60;
const countdownElement = document.getElementById('countdown');
var updateCountdown = () => {
    const minutes = Math.floor(timeInSeconds / 60);
    const seconds = timeInSeconds % 60;

    countdownElement.textContent = `${String(minutes).padStart(2, '0')}:${String(seconds).padStart(2, '0')}`;

    if (timeInSeconds > 0) {
        timeInSeconds--;
    } else {
        clearInterval(countdownInterval);
        countdownElement.style.color = "red";
    }
};
var countdownInterval;
function phoneAuth() {
    appVerificationDisabledForTesting = true
    var a = document.getElementById("floating_phone").value;
    var b = "+84";
    var number = b + a.substring(1);

    //this.window.confirmationResult = confirmationResult;
    //coderesult = confirmationResult;
    window.scrollTo({
        top: 0,
        behavior: 'smooth'
    });

    firebase.auth().signInWithPhoneNumber(number, window.recaptchaVerifier).then(function (confirmationResult) {
        this.window.confirmationResult = confirmationResult;
        coderesult = confirmationResult;
        window.scrollTo({
            top: 0,
            behavior: 'smooth'
        });
        timeInMinutes = 3;
        timeInSeconds = timeInMinutes * 60;
        document.getElementById("countdown").innerHTML = "03:00";
        countdownInterval = setInterval(updateCountdown, 1000);
        document.getElementById("phonenumber").innerHTML = a;
        

        if (document.getElementById("toast-success").classList.contains("animate-[fade-out_2s_ease-out_1s_1_forwards]"))
            document.getElementById("toast-success").classList.remove("animate-[fade-out_2s_ease-out_1s_1_forwards]");

        if (document.getElementById("animateDiv").classList.contains("animate-[reduce_2s_ease-in-out_0s_1_forwards]"))
            document.getElementById("animateDiv").classList.remove("animate-[reduce_2s_ease-in-out_0s_1_forwards]");

        void document.getElementById("toast-success").offsetWidth;
        void document.getElementById("animateDiv").offsetWidth;

        document.getElementById("toast-er-message").innerHTML = "OTP has expired. Please resend it";

        document.getElementById("toast-success").classList.add("animate-[fade-out_2s_ease-out_1s_1_forwards]");
        document.getElementById("animateDiv").classList.add("animate-[reduce_2s_ease-in-out_0s_1_forwards]");

        if (document.getElementById("toast-success").classList.contains("hidden"))
            document.getElementById("toast-success").classList.remove("hidden");
        document.getElementById("toast-success").classList.add("block");

        document.getElementById("notverify").style.display = "none";
        document.getElementById("verify").style.display = "block";
       
    }).catch(function (error) {
        console.log(error);
        document.getElementById("toast-er-message").innerHTML = "An error has occured. Please try again";
        document.getElementById("toast-danger").classList.remove("hidden");
        document.getElementById("toast-danger").classList.add("block");
    })
     

}




function codeverify() {
    if(document.getElementById("countdown").innerHTML == "00:00") {

        if (document.getElementById("toast-danger").classList.contains("animate-[fade-out_2s_ease-out_1s_1_forwards]"))
            document.getElementById("toast-danger").classList.remove("animate-[fade-out_2s_ease-out_1s_1_forwards]");

        if (document.getElementById("animateDiv1").classList.contains("animate-[reduce_2s_ease-in-out_0s_1_forwards]"))
            document.getElementById("animateDiv1").classList.remove("animate-[reduce_2s_ease-in-out_0s_1_forwards]");

        void document.getElementById("toast-danger").offsetWidth;
        void document.getElementById("animateDiv1").offsetWidth;

        document.getElementById("toast-er-message").innerHTML = "OTP has expired. Please resend it";

        document.getElementById("toast-danger").classList.add("animate-[fade-out_2s_ease-out_1s_1_forwards]");
        document.getElementById("animateDiv1").classList.add("animate-[reduce_2s_ease-in-out_0s_1_forwards]");

        if (document.getElementById("toast-danger").classList.contains("hidden"))
            document.getElementById("toast-danger").classList.remove("hidden");
        document.getElementById("toast-danger").classList.add("block");
        return;
    }
    var code = document.getElementById("verificationCode").value;
    coderesult.confirm(code).then(function (result) {
        //alert("Message Verified");
        var user = result.user;
        document.getElementById("form").submit();
    }).catch(function (error) {
        if (document.getElementById("toast-danger").classList.contains("animate-[fade-out_2s_ease-out_1s_1_forwards]"))
            document.getElementById("toast-danger").classList.remove("animate-[fade-out_2s_ease-out_1s_1_forwards]");

        if (document.getElementById("animateDiv1").classList.contains("animate-[reduce_2s_ease-in-out_0s_1_forwards]"))
            document.getElementById("animateDiv1").classList.remove("animate-[reduce_2s_ease-in-out_0s_1_forwards]");

        void document.getElementById("toast-danger").offsetWidth;
        void document.getElementById("animateDiv1").offsetWidth;

        document.getElementById("toast-er-message").innerHTML = "An error has occured. Please try again";

        document.getElementById("toast-danger").classList.add("animate-[fade-out_2s_ease-out_1s_1_forwards]");
        document.getElementById("animateDiv1").classList.add("animate-[reduce_2s_ease-in-out_0s_1_forwards]");

        if (document.getElementById("toast-danger").classList.contains("hidden"))
            document.getElementById("toast-danger").classList.remove("hidden");
        document.getElementById("toast-danger").classList.add("block");
    })
}