using System;
using System.Text;

namespace HS.Utils
{
    public static class ExceptionUtils
    {
        /// <summary>
        /// 예외 정보를 사람이 읽기 쉬운 문자열로 변환합니다.
        /// 
        /// [Tree == false]
        /// - 단일 라인 포맷
        ///   "{message1 -> message2 -> ...} [{fileName} ({methodName}) {line}:{column}]"
        /// 
        /// [Tree == true]
        /// - 멀티라인 트리 포맷 (메시지만 출력)
        ///   TopMessage
        ///     -> InnerMessage
        ///       -> InnerInnerMessage
        /// 
        /// - 내부 예외(InnerException)는 깊이만큼 순차적으로 출력됩니다.
        /// - 파일명/함수명/줄/열 정보는 StackTrace + PDB(디버그 심볼)가 있을 때만 제공됩니다.
        /// </summary>
        /// <param name="ex">처리할 예외</param>
        /// <returns>포맷된 예외 메시지(실패 시 ex.Message 또는 null)</returns>
        public static string ToMessage(this Exception ex, bool Tree = false)
        {
            if (ex == null) return null;

            try
            {
                if (Tree) return BuildTree(ex, 0);

                // 단일 라인: 최상위 예외 메시지 + 내부 예외 메시지를 깊이만큼 연결
                var sbMsg = new StringBuilder();
                sbMsg.Append(ex.Message);

                var inner = ex.InnerException;
                while (inner != null)
                {
                    if (!string.IsNullOrWhiteSpace(inner.Message))
                    {
                        sbMsg.Append(" -> ").Append(inner.Message);
                    }
                    inner = inner.InnerException;
                }

                var message = sbMsg.ToString();

                // StackTrace에서 가장 첫 번째 유효한 프레임 사용
                var st = new System.Diagnostics.StackTrace(ex, true);
                var frame = st.FrameCount > 0 ? st.GetFrame(0) : null;

                if (frame == null)
                    return message;

                var fileName = System.IO.Path.GetFileName(frame.GetFileName() ?? string.Empty);
                var lineNo = frame.GetFileLineNumber();
                var column = frame.GetFileColumnNumber();

                var method = frame.GetMethod();
                var methodName = method != null ? method.Name : string.Empty;

                // 요청 포맷: {message} [{fileName} ({methodName}) {line}:{column}]
                // methodName이 없으면 () 생략
                if (!string.IsNullOrEmpty(methodName))
                {
                    return $"{message} [{fileName} ({methodName}) {lineNo}:{column}]";
                }
                else
                {
                    return $"{message} [{fileName} {lineNo}:{column}]";
                }
            }
            catch
            {
                // 예외 메시지 생성 중 오류가 발생하면 최소 메시지만 반환
                return ex.Message;
            }
        }

        /// <summary>
        /// 예외 및 InnerException을 재귀적으로 순회하여 문자열로 직렬화합니다.
        /// 
        /// [Tree == false]
        /// - 단일 라인 체인 포맷
        ///   "{seg0} -> {seg1} -> {seg2}"
        ///   seg = "{message} [{fileName} {line}:{column}]" (정보가 없으면 message만 출력)
        /// 
        /// [Tree == true]
        /// - 멀티라인 트리 포맷 (각 노드가 동일한 seg 포맷 사용)
        ///   seg0
        ///     -> seg1
        ///       -> seg2
        /// 
        /// - 각 예외는 동일한 규칙으로 포맷되며, InnerException 깊이에 따라 들여쓰기 됩니다.
        /// </summary>
        /// <param name="ex">처리할 예외</param>
        /// <returns>포맷된 예외 메시지(실패 시 ex.Message 또는 null)</returns>
        public static string ToMessageAlt(this Exception ex, bool Tree = false)
        {
            if (ex == null) return null;
            try
            {
                return Tree ? BuildTree(ex, 0) : FormatRecursive(ex);
            }
            catch
            {
                return ex.Message;
            }
        }
        
        #region Private
        /// <summary>
        /// 세그먼트를 트리 형태로 출력
        /// </summary>
        /// <param name="e"></param>
        /// <param name="depth"></param>
        /// <returns></returns>
        private static string BuildTree(Exception e, int depth)
        {
            if (e == null) return string.Empty;

            var seg = FormatOne(e);
            var indent = new string(' ', depth * 2);
            var line = depth == 0
                ? seg
                : $"{indent}-> {seg}";

            if (e.InnerException == null) return line;

            var child = BuildTree(e.InnerException, depth + 1);
            return string.IsNullOrWhiteSpace(child) ? line : $"{line}{Environment.NewLine}{child}";
        }
        
        private static string FormatRecursive(Exception e)
        {
            var current = FormatOne(e);
            if (e?.InnerException == null) return current;

            var inner = FormatRecursive(e.InnerException);
            if (string.IsNullOrWhiteSpace(inner)) return current;

            return $"{current} -> {inner}";
        }
        
        private static string FormatOne(Exception e)
        {
            if (e == null) return string.Empty;

            try
            {
                var msg = e.Message;

                // 가장 첫 번째 유효 프레임 기준으로 파일/라인/컬럼 추출
                var st = new System.Diagnostics.StackTrace(e, true);
                var frame = st.FrameCount > 0 ? st.GetFrame(0) : null;

                if (frame == null)
                    return msg;

                var fileName = System.IO.Path.GetFileName(frame.GetFileName() ?? string.Empty);
                var lineNo = frame.GetFileLineNumber();
                var column = frame.GetFileColumnNumber();

                var method = frame.GetMethod();
                var methodName = method != null ? method.Name : string.Empty;

                // fileName이 비어있거나 line/column이 0인 경우도 있을 수 있으므로,
                // 최소한 msg는 항상 반환하도록 처리
                if (string.IsNullOrWhiteSpace(fileName) || (lineNo == 0 && column == 0))
                    return msg;

                if (!string.IsNullOrEmpty(methodName))
                {
                    return $"{msg} [{fileName} ({methodName}) {lineNo}:{column}]";
                }
                else
                {
                    return $"{msg} [{fileName} {lineNo}:{column}]";
                }
            }
            catch
            {
                return e.Message;
            }
        }
        #endregion
    }
}