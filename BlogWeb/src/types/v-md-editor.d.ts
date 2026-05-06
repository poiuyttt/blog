declare module "@kangc/v-md-editor" {
  import type { Plugin } from "vue";
  const VMdEditor: Plugin & {
    use(plugin: unknown, options?: Record<string, unknown>): void;
  };
  export default VMdEditor;
}

declare module "@kangc/v-md-editor/lib/preview" {
  import type { Component } from "vue";
  interface VMdPreviewProps {
    text: string;
    [key: string]: unknown;
  }
  const VMdPreview: Component<VMdPreviewProps> & {
    use(plugin: unknown, options?: Record<string, unknown>): void;
  };
  export default VMdPreview;
}

declare module "@kangc/v-md-editor/lib/theme/github" {
  const githubTheme: {
    install(vm: unknown, options?: { Hljs?: unknown }): void;
  };
  export default githubTheme;
}

declare module "@kangc/v-md-editor/lib/plugins/line-number/index" {
  const createLineNumberPlugin: (() => object) & { default?: () => object };
  export default createLineNumberPlugin;
}
