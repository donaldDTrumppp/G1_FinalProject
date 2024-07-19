function detectClassChange(mutationsList) {
    for (let mutation of mutationsList) {
        if (mutation.attributeName === 'class') {
            const element = mutation.target;
            if (element.classList.contains('hidden')) {
                console.log('Element is now hidden');
            } else if (element.classList.contains('block')) {
                console.log('Element is now block');
            }
        }
    }
}

function moveDiv() {
    const div1 = document.getElementById('recaptcha-container');
    const div2 = document.getElementById('div2');
    const div3 = document.getElementById('div3');

    if (document.getElementById("notverify").style.display == "none") {
        div2.insertBefore(div1, div2.firstChild);
        currentParentIndex = 3;
    } else {
        div3.insertBefore(div1, div3.children[1]);
        currentParentIndex = 2;
    }
}

// Tạo một MutationObserver để theo dõi sự thay đổi thuộc tính của phần tử
const observer = new MutationObserver(detectClassChange);

// Bắt đầu quan sát phần tử với các tùy chọn được chỉ định
const targetNode = document.getElementById('verify');
const config = { attributes: true, attributeFilter: ['class'] };
observer.observe(targetNode, config);

window.onload = function () {
    moveDiv();
    window.recaptchaVerifier = new firebase.auth.RecaptchaVerifier("recaptcha-container", {

    });
    recaptchaVerifier.render();

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

var isZero = false;
var isOne = false

function render() {
    /*
    var elementWithX = document.getElementById("recaptcha-container");
    var elementWithY = document.getElementById("recaptcha-container-1");
    console.log(elementWithX);
    console.log(elementWithY);

    elementWithX.removeAttribute("id");
    elementWithX.id = "recaptcha-container-1"

    elementWithY.id = "recaptcha-container";
    */

    moveDiv();

}


var coderesult;

function swtch2() {
    document.getElementById("notverify").style.display = "none";
    document.getElementById("verify").style.display = "block";
    timeInMinutes = 3;
    timeInSeconds = timeInMinutes * 60;
    document.getElementById("countdown").innerHTML = "03:00";
    countdownInterval = setInterval(updateCountdown, 1000);
    render();
}

function resend() {
    var a = document.getElementById("floating_phone").value;
    var b = "+84";
    var number = b + a.substring(1);

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

function swtch() {
    clearInterval(countdownInterval);
    document.getElementById("notverify").style.display = "block";
    document.getElementById("verify").style.display = "none";
    window.scrollTo({
        top: 0,
        behavior: 'smooth'
    });
    render();



   
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
    console.log("23");
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

        render();
       
    }).catch(function (error) {
        console.log("236");
        console.log(error);
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