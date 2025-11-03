import { definePreset } from "@primeuix/themes";
import Aura from "@primeuix/themes/aura";

export const queryGlassTheme = definePreset(Aura, {
    semantic: {
        primary: {
            '50': '{violet-50}',
            '100': '{violet-200}',
            '200': '{violet-300}',
            '300': '{violet-400}',
            '400': '{violet-500}',
            '500': '{violet-600}',
            '600': '{violet-700}',
            '700': '{violet-800}',
            '800': '{violet-900}',
            '900': '{violet-950}',
        }
    }
})