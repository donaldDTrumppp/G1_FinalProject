/** @type {import('tailwindcss').Config} */
module.exports = {
    content: [
        './Pages/**/*.cshtml',
        './Views/**/*.cshtml',
        './wwwroot/js/*.js',
    ],
    theme: {
        extend: {
            keyframes: {
                "fade-out": {
                    "0%": {
                        opacity: 1
                    },

                    "100%": {
                        opacity: 0
                    }
                },
                "fade-in": {
                    "0%": {
                        opacity: 0
                    },
                    "100%": {
                        opacity: 1
                    }
                },
                "slide-in-right": {
                    "0%": {
                        visibility: "visible",
                        transform: "translate3d(100%, 0, 0)",
                    },
                    "100%": {
                        transform: "translate3d(0, 0, 0)",
                    },
                },
                "reduce": {
                    "0%": {
                        width: "100%"
                    },

                    "100%": {
                        width: "0%"
                    }
                }
            },
            animation: {
                fadeout: 'fade-out 1s ease-out 0.25s 1 forwards',
                fadein: 'fade-in 1s ease-in-out 0.25s 1',
                slideinright: 'slide-in-right 1s ease-in-out 0.25s 1',
                reduce: 'reduce 1s ease-in-out 0.25s 1 forwards'
            }
        },
    },
    plugins: [
        require('flowbite/plugin'),
        require('preline/plugin'),
        //require("tailwindcss-animate"),

    ]
}