import { PropType } from 'vue';
import type Color from '../color';
declare const _default: import("vue").DefineComponent<{
    color: {
        type: PropType<Color>;
        required: true;
    };
    vertical: {
        type: BooleanConstructor;
        default: boolean;
    };
}, {
    thumb: any;
    bar: any;
    thumbLeft: import("vue").Ref<number>;
    thumbTop: import("vue").Ref<number>;
    background: any;
    handleClick: (event: Event) => void;
    update: () => void;
}, unknown, {}, {}, import("vue").ComponentOptionsMixin, import("vue").ComponentOptionsMixin, Record<string, any>, string, import("vue").VNodeProps & import("vue").AllowedComponentProps & import("vue").ComponentCustomProps, Readonly<{
    color: Color;
    vertical: boolean;
} & {}>, {
    vertical: boolean;
}>;
export default _default;
