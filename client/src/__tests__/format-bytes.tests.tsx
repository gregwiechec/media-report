import { formatBytes } from "./../format-bytes";

describe("When value is NOT set", () => {
    test("it should return 0", () => {
        expect(formatBytes(0)).toBe("0 Bytes");
    });
});

describe("When value is set", () => {
   describe("and value is less then a kilobyte", () => {
       test("it should return value in butes", () => {
           expect(formatBytes(100)).toBe("100 Bytes");
       });
   });
});
