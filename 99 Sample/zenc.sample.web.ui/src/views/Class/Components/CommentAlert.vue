<template>
    <div class="toast fade show p-2 mt-2" :class="getColor(color)">
        <div class="toast-header bg-transparent border-0">
            <i class="me-2" :class="getIcon(icon, iconColor)"></i>
            <span class="me-auto font-weight-bold" :class="getTextColor(color)">
                {{ data.Title }}
            </span>
            <small :class="getTextColor(color)">{{ data.MsgDate }}</small>
            <i class="fas fa-times text-md ms-3 cursor-pointer" :class="getTextColor(color)" v-show="data.OnClick"
                @click="data.OnClick(false)"></i>

        </div>
        <hr class="horizontal dark m-0" :class="getHrColor(color)" />
        <div class="toast-body" :class="getTextColor(color)" @click="data.OnMsgClick">{{ data.Msg }}</div>
        <hr class="horizontal dark m-0" :class="getHrColor(color)" v-show="data.OnClick" />
        <div class="text-center" v-show="data.OnClick">
            <svg v-if="isPlay" xmlns="http://www.w3.org/2000/svg" @click="isPlay = false; data.OnClick(true)" width="24"
                height="24" fill="currentColor" class="cursor-pointer" viewBox=" 0 0 16 16">
                <path
                    d="M10.804 8 5 4.633v6.734L10.804 8zm.792-.696a.802.802 0 0 1 0 1.392l-6.363 3.692C4.713 12.69 4 12.345 4 11.692V4.308c0-.653.713-.998 1.233-.696l6.363 3.692z" />
            </svg>
            <svg v-else xmlns="http://www.w3.org/2000/svg" @click="data.OnClick(false)" width="24" height="24"
                fill="red" class="cursor-pointer" viewBox="0 0 16 16">
                <path
                    d="M3.5 5A1.5 1.5 0 0 1 5 3.5h6A1.5 1.5 0 0 1 12.5 5v6a1.5 1.5 0 0 1-1.5 1.5H5A1.5 1.5 0 0 1 3.5 11V5zM5 4.5a.5.5 0 0 0-.5.5v6a.5.5 0 0 0 .5.5h6a.5.5 0 0 0 .5-.5V5a.5.5 0 0 0-.5-.5H5z" />
            </svg>
        </div>
    </div>
</template>

<script>

export default {
    name: "VsudSnackbar",
    props: {
        
        icon: {
            type: [String, Object],
            component: {
                type: String,
            },
            color: {
                type: String,
            },
            default: () => ({
                color: "success",
            }),
        },
        color: {
            type: String,
            default: "success",
        },
        closeHandler: {
            type: Function,
            default: null,
        },

        data: { type: Object }
    },
    data: function () {
        return {
            isPlay: true
        }
    },
    methods: {
        getColor: (color) => {
            let colorValue;

            if (color === "white") {
                colorValue = "bg-white";
            } else {
                colorValue = `bg-gradient-${color}`;
            }

            return colorValue;
        },
        getIcon(icon) {
            if (icon && typeof icon === "string") {
                return icon;
            } else if (icon && typeof icon === "object") {
                return `${icon.component} text-${icon.color}`;
            } else {
                return null;
            }
        },
        getTextColor: (color) => (color === "white" ? "text-dark" : "text-white"),
        getHrColor: (color) => (color === "white" ? "dark" : "light"),
    },
    mounted: function (){
        
        this.data.ReloadMsg = (msg) => {
            this.data.Msg = msg;
        }
    }
};
</script>
