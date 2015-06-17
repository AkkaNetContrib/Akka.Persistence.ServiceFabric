using Akka.Persistence.ServiceFabric.Tests;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Akka.Persistence.ServiceFabric.X64TestRunner
{
    class Program
    {
        static void Main(string[] args)
        {
            RunTest(test => test.Journal_should_not_replay_messages_if_count_limit_equals_zero());
            RunTest(test => test.Journal_should_not_replay_messages_if_lower_sequence_number_bound_is_greater_than_upper_sequence_number_bound());
            RunTest(test => test.Journal_should_replay_all_messages());
            RunTest(test => test.Journal_should_replay_a_single_if_lower_sequence_number_bound_equals_upper_sequence_number_bound());
            RunTest(test => test.Journal_should_replay_a_single_message_if_count_limit_is_equal_one());
            RunTest(test => test.Journal_should_replay_logically_deleted_messages_with_deleted_flag_set_on_range_deletion());
            RunTest(test => test.Journal_should_replay_messages_using_an_upper_sequence_number_bound());
            RunTest(test => test.Journal_should_replay_messages_using_a_count_limit());
            RunTest(test => test.Journal_should_replay_messages_using_a_lower_sequence_number_bound());
            RunTest(test => test.Journal_should_replay_messages_using_lower_and_upper_sequence_number_bound());
            RunTest(test => test.Journal_should_replay_messages_using_lower_and_upper_sequence_number_bound_and_count_limit());
            RunTest(test => test.Journal_should_return_a_highest_sequence_number_equal_zero_if_actor_did_not_written_any_messages_yet());
            RunTest(test => test.Journal_should_return_a_highest_seq_number_greater_than_zero_if_actor_has_already_written_messages_and_message_log_is_not_empty());


            Console.ReadLine();
        }

        private static void RunTest(Expression<Action<ServiceFabricJournalSpec>> x)
        {
            Console.ForegroundColor = ConsoleColor.White;
            var name = x.Body.ToString();
            var action = x.Compile();
            try
            {
                var test = new ServiceFabricJournalSpec();
                action(test);
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("{0} Success..",name);
            }
            catch
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("{0} Failed..",name);
            }
        }
    }
}
